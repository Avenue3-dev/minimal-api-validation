namespace EndpointValidator.Internal;

using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

internal static class RequestBody
{
    private record ValidatorInfo(Type ValidatorType, Type ContextType);

    private static ConcurrentDictionary<Type, ValidatorInfo> BodyValidatorCache { get; } = new();

    public static async Task<List<ValidationFailure>> HandleAsync(
        ParameterAttributeInfo arg,
        HttpContext context,
        MinimalApiValidationOptions options)
    {
        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
        var json = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;

        if (string.IsNullOrEmpty(json))
        {
            return [new ValidationFailure("body", "A body is required but was null or empty.")];
        }

        var jsonOptions = options.JsonSerializerOptions
            ?? context.RequestServices.GetService<JsonOptions>()?.SerializerOptions
            ?? context.RequestServices.GetService<IOptions<JsonOptions>>()?.Value.SerializerOptions;

        if (!TryDeserialize(json, arg.ParameterType, jsonOptions, out var deserialized))
        {
            return deserialized.errors;
        }

        var value = deserialized.value;

        if (!BodyValidatorCache.TryGetValue(arg.ParameterType, out var info))
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(arg.ParameterType);
            var validationContextType = typeof(ValidationContext<>).MakeGenericType(arg.ParameterType);
            info = new ValidatorInfo(validatorType, validationContextType);
            BodyValidatorCache.TryAdd(arg.ParameterType, info);
        }

        var validator = context.RequestServices.GetService(info.ValidatorType);
        if (validator is null)
        {
            return value is not null && options.FallbackToDataAnnotations
                ? ValidateDataAnnotations(value)
                : [];
        }

        var validationContext = Activator.CreateInstance(info.ContextType, value) as IValidationContext
            ?? throw new InvalidOperationException("Failed to create validation context.");

        var result = await ((IValidator) validator).ValidateAsync(validationContext);
        return result.Errors;
    }

    private static List<ValidationFailure> ValidateDataAnnotations(object value)
    {
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(value);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(value, context, results, validateAllProperties: true);

        return isValid
            ? []
            : results.Select(x => new ValidationFailure(x.MemberNames.FirstOrDefault() ?? "", x.ErrorMessage)).ToList();
    }

    private static bool TryDeserialize(
        string json,
        Type type,
        JsonSerializerOptions? options,
        out (object? value, List<ValidationFailure> errors) result)
    {
        try
        {
            var value = JsonSerializer.Deserialize(json, type, options);
            result = (value, []);
            return true;
        }
        catch (Exception ex) when (ex is not ValidationException)
        {
            if (ex is JsonException jex)
            {
                result = (null, [new ValidationFailure(jex.Path, "The JSON value could not be converted.")]);
                return false;
            }

            result = (null, [new ValidationFailure("body", "Error binding request object.")]);
            return false;
        }
    }
}
