namespace A3.MinimalApiValidation.Internal.Filter;

using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

internal class RequestBodyValidationFilter<T> : IEndpointFilter
    where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var options = context.HttpContext.RequestServices.GetService<EndpointValidatorOptions>()
            ?? Utils.DefaultOptions;
        
        var logger = (context.HttpContext.RequestServices.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance)
            .CreateLogger(options.LoggerCategoryName);

        var value = context.Arguments.FirstOrDefault(a => a?.GetType() == typeof(T)) as T;
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        
        logger.LogInformation("Validating endpoint: {EndpointDisplayName}.", context.HttpContext.GetEndpoint()?.DisplayName ?? "[NO DISPLAY NAME]");

        // validate the single model
        if (value is not null)
        {
            logger.LogDebug("Validating single model of type {Type}", typeof(T).Name);
            
            var result = await (validator is null
                ? Utils.ValidateDataAnnotationsAsync(value, options)
                : validator.ValidateAsync(value));

            if (result.IsValid)
            {
                logger.LogInformation("Validation passed.");
                return await next(context);
            }

            logger.LogWarning(
                "Validation failed with {numErrors} error(s):{newline}{errors}",
                result.Errors.Count,
                Environment.NewLine,
                string.Join(Environment.NewLine, result.Errors.Select(e => $"- {e.PropertyName}: {e.ErrorMessage}"))
            );
            
            return Results.ValidationProblem(result.ToDictionary());
        }

        // otherwise, fallback to IEnumerable<T> validation
        if (context.Arguments.FirstOrDefault(a => a?.GetType().IsAssignableTo(typeof(IEnumerable<T>)) == true) is not IEnumerable<T> collection)
        {
            throw new InvalidOperationException($"Could not find argument that matches {nameof(T)} to validate.");
        }
        
        logger.LogDebug("Validating models array of type {Type}", typeof(T).Name);
        
        var results = await Utils.ValidateCollectionAsync(
            collection,
            validator,
            options,
            val => new ValidationContext<T>(val));

        if (results.Errors.Count != 0)
        {
            logger.LogWarning(
                "Validation failed with {numErrors} error(s):{newline}{errors}",
                results.Errors.Count,
                Environment.NewLine,
                string.Join(Environment.NewLine, results.Errors.Select(e => $"- {e.PropertyName}: {e.ErrorMessage}"))
            );
            
            return Results.ValidationProblem(
                new ValidationResult(results.Errors).ToDictionary(),
                results.Detail);
        }

        logger.LogInformation("Validation passed.");
        return await next(context);
    }
}
