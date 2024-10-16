namespace EndpointValidator.Internal.Middleware;

using System.Collections.Concurrent;
using System.Text;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

internal static class Body
{
    private static ConcurrentDictionary<Type, BodyParameterInfo> BodyValidatorCache { get; } = new();

    public static async Task<BodyValidationResult> HandleAsync(
        ParameterAttributeInfo arg,
        HttpContext context,
        EndpointValidatorOptions options)
    {
        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
        var json = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;

        if (string.IsNullOrEmpty(json))
        {
            return new ValidationFailure("body", "A body is required but was null or empty.");
        }

        var jsonOptions = options.JsonSerializerOptions
            ?? context.RequestServices.GetService<JsonOptions>()?.SerializerOptions
            ?? context.RequestServices.GetService<IOptions<JsonOptions>>()?.Value.SerializerOptions;

        if (!Utils.TryDeserialize(json, arg.ParameterType, jsonOptions, out var value, out var errors))
        {
            return errors;
        }

        if (value is null)
        {
            return new ValidationFailure("body", "A body is required but was null or empty.");
        }

        if (!BodyValidatorCache.TryGetValue(arg.ParameterType, out var info))
        {
            info = new BodyParameterInfo(arg.ParameterType);
            BodyValidatorCache.TryAdd(arg.ParameterType, info);
        }

        var validator = context.RequestServices.GetService(info.ValidatorType);

        // validate the single model
        if (!info.IsEnumerable)
        {
            var result = await (validator is null
                ? Utils.ValidateDataAnnotationsAsync(value, options)
                : ((IValidator) validator).ValidateAsync(info.CreateValidationContext(value)));

            return result.Errors;
        }

        // otherwise, fallback to IEnumerable<T> validation
        if (value is not IEnumerable<object> collection)
        {
            throw new InvalidOperationException($"Could not find argument that matches {arg.ParameterType} to validate.");
        }
        
        return await Utils.ValidateCollectionAsync(
            collection,
            validator as IValidator,
            options,
            info.CreateValidationContext);
    }
}
