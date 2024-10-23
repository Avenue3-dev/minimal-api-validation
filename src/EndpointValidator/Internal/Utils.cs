namespace EndpointValidator.Internal;

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using EndpointValidator.Internal.Middleware;
using FluentValidation;
using FluentValidation.Results;

internal static class Utils
{
    public static EndpointValidatorOptions DefaultOptions { get; } = new();
    
    public static Task<ValidationResult> ValidateDataAnnotationsAsync(object? value, EndpointValidatorOptions options)
    {
        return Task.FromResult(ValidateDataAnnotations(value, options));
    }

    private static ValidationResult ValidateDataAnnotations(object? value, EndpointValidatorOptions options)
    {
        if (!options.FallbackToDataAnnotations)
        {
            return new ValidationResult();
        }

        if (value is null)
        {
            return new ValidationResult([new ValidationFailure("requestBody", "Value cannot be null.")]);
        }

        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(value);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(value, context, results, validateAllProperties: true);

        var errors = isValid
            ? []
            : results.Select(x => new ValidationFailure(x.MemberNames.FirstOrDefault() ?? "", x.ErrorMessage)).ToList();

        return new ValidationResult(errors);
    }

    public static bool TryDeserialize(
        string json,
        Type type,
        JsonSerializerOptions? options,
        out object? value,
        out List<ValidationFailure> errors)
    {
        value = null;
        errors = [];

        try
        {
            value = JsonSerializer.Deserialize(json, type, options);
            return true;
        }
        catch (Exception ex) when (ex is not ValidationException)
        {
            if (ex is JsonException jex)
            {
                errors.Add(new ValidationFailure(jex.Path, "The JSON value could not be converted."));
            }
            else if (ex.InnerException is JsonException jexInner)
            {
                errors.Add(new ValidationFailure(jexInner.Path, "The JSON value could not be converted."));
            }
            else
            {
                errors.Add(new ValidationFailure("requestBody", "Error binding request object."));
            }

            return false;
        }
    }

    public static bool IsEnumerable(Type type, [NotNullWhen(true)] out Type? firstUnderlyingType)
    {
        if (!typeof(IEnumerable).IsAssignableFrom(type))
        {
            firstUnderlyingType = null;
            return false;
        }

        if (type is {IsInterface: true, IsGenericType: true}
            && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            firstUnderlyingType = type.GetGenericArguments()[0];
            return true;
        }

        firstUnderlyingType = type
            .GetInterfaces()
            .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            ?.GetGenericArguments()[0];

        return firstUnderlyingType is not null;
    }

    public static async Task<BodyValidationResult> ValidateCollectionAsync<T>(
        IEnumerable<T> collection,
        IValidator? validator,
        EndpointValidatorOptions options,
        Func<T, IValidationContext> getContext
    )
    {
        var results = new List<ValidationFailure>();
        var failedIndexes = new List<int>();
        foreach (var item in collection.Select((v, i) => new {Index = i, Value = v}))
        {
            var itemResult = await (validator is null
                ? ValidateDataAnnotationsAsync(item.Value, options)
                : validator.ValidateAsync(getContext(item.Value)));

            if (itemResult.IsValid)
            {
                continue;
            }

            var failures = itemResult.Errors.Select(e => new ValidationFailure($"item[{item.Index}].{e.PropertyName}", e.ErrorMessage));
            results.AddRange(failures);
            failedIndexes.Add(item.Index);
        }

        return new BodyValidationResult(
            results,
            $"{failedIndexes.Count} items in the array failed validation, indexes: {string.Join(", ", failedIndexes)}");
    }
}
