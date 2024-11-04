namespace A3.MinimalApiValidation.Binders;

using System.Reflection;
using A3.MinimalApiValidation.Exceptions;
using A3.MinimalApiValidation.Internal;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

public class FromQuery<T> : IRouteDelegateBinder<FromQuery<T>> where T : class
{
    private FromQuery(T value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the instance of <typeparam name="T"/> that was bound from the query string.
    /// </summary>
    public T Value { get; }

    public static async ValueTask<FromQuery<T>?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var info = QueryTypeInfo.Get<T>();

        var options = context.RequestServices.GetService<EndpointValidatorOptions>()
            ?? EndpointValidatorOptions.Default;

        var logger = context.GetLogger<FromQuery<T>>();

        var validator = context.RequestServices.GetService<IValidator<T>>();

        logger.Info_EndpointDetails(context.GetEndpoint()?.DisplayName ?? "[NO DISPLAY NAME]");
        logger.Debug_ValidatingModel(info.Type.Name);

        var query = context.Request.Query;
        var queryParams = info.CreateInstance(query) as T ?? throw new InvalidOperationException("Failed to create instance of type.");

        if (options.PreferExplicitRequestModelValidation)
        {
            return new FromQuery<T>(queryParams);
        }

        var result = await (validator is null
            ? Utils.ValidateDataAnnotationsAsync(queryParams, options)
            : validator.ValidateAsync(queryParams));

        if (!result.IsValid)
        {
            // validation failure
            logger.Info_ValidationFailed(result.Errors.Count, result.Errors);
            throw new BinderValidationFailedException(result.Errors);
        }

        // ok
        logger.Info_ValidationPassed();
        return new FromQuery<T>(queryParams);
    }
}
