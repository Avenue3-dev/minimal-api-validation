namespace A3.MinimalApiValidation.Internal.Filter;

using A3.MinimalApiValidation.Binders;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

internal class RequestModelValidationFilter<T> : IEndpointFilter
    where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var type = typeof(T);

        var options = context.HttpContext.RequestServices.GetService<EndpointValidatorOptions>()
            ?? EndpointValidatorOptions.Default;

        var logger = context.HttpContext.GetLogger<RequestModelValidationFilter<T>>();

        // is there an argument that matches T?
        // if not, is there one that matches one of our custom binders?
        var value = context.Arguments.FirstOrDefault(a => a?.GetType() == type) as T
            ?? (context.Arguments.FirstOrDefault(a => a?.GetType() == typeof(FromQuery<T>)) as FromQuery<T>)?.Value;

        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

        logger.Info_EndpointDetails(context.HttpContext.GetEndpoint()?.DisplayName ?? "[NO DISPLAY NAME]");

        // validate the single model
        if (value is not null)
        {
            logger.Debug_ValidatingModel(type.Name);

            var result = await (validator is null
                ? Utils.ValidateDataAnnotationsAsync(value, options)
                : validator.ValidateAsync(value));

            if (result.IsValid)
            {
                logger.Info_ValidationPassed();
                return await next(context);
            }

            logger.Info_ValidationFailed(result.Errors.Count, result.Errors);

            return Results.ValidationProblem(result.ToDictionary());
        }

        // otherwise, fallback to IEnumerable<T> validation
        if (context.Arguments.FirstOrDefault(a => a?.GetType().IsAssignableTo(typeof(IEnumerable<T>)) == true) is not IEnumerable<T> collection)
        {
            throw new InvalidOperationException($"Could not find argument that matches {nameof(T)} to validate.");
        }

        logger.Debug_ValidatingModelsArray(type.Name);

        var results = await Utils.ValidateCollectionAsync(
            collection,
            validator,
            options,
            val => new ValidationContext<T>(val));

        if (results.Errors.Count != 0)
        {
            logger.Info_ValidationFailed(results.Errors.Count, results.Errors);

            return Results.ValidationProblem(
                new ValidationResult(results.Errors).ToDictionary(),
                results.Detail);
        }

        logger.Info_ValidationPassed();
        return await next(context);
    }
}
