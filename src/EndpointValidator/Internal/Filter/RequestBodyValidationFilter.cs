namespace EndpointValidator.Internal.Filter;

using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

internal class RequestBodyValidationFilter<T> : IEndpointFilter
    where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var options = context.HttpContext.RequestServices.GetService<EndpointValidatorOptions>()
            ?? Utils.DefaultOptions;

        var value = context.Arguments.FirstOrDefault(a => a?.GetType() == typeof(T)) as T;
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

        // validate the single model
        if (value is not null)
        {
            var result = await (validator is null
                ? Utils.ValidateDataAnnotationsAsync(value, options)
                : validator.ValidateAsync(value));

            if (result.IsValid)
            {
                return await next(context);
            }

            return Results.ValidationProblem(result.ToDictionary());
        }

        // otherwise, fallback to IEnumerable<T> validation
        if (context.Arguments.FirstOrDefault(a => a?.GetType().IsAssignableTo(typeof(IEnumerable<T>)) == true) is not IEnumerable<T> collection)
        {
            throw new InvalidOperationException($"Could not find argument that matches {nameof(T)} to validate.");
        }
        
        var results = await Utils.ValidateCollectionAsync(
            collection,
            validator,
            options,
            val => new ValidationContext<T>(val));

        if (results.Errors.Count != 0)
        {
            return Results.ValidationProblem(
                new ValidationResult(results.Errors).ToDictionary(),
                results.Detail);
        }

        return await next(context);
    }
}
