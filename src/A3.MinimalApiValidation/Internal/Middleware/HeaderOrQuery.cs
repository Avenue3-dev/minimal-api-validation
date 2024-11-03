namespace A3.MinimalApiValidation.Internal.Middleware;

using System.ComponentModel.DataAnnotations;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

internal static class HeaderOrQuery
{
    public static IEnumerable<ValidationFailure> Handle(ParameterAttributeInfo arg, HttpContext context)
    {
        var value = arg.IsHeader
            ? context.Request.Headers[arg.Name].FirstOrDefault()
            : context.Request.Query[arg.Name].FirstOrDefault();

        var message = arg.IsHeader
            ? $"{arg.Name} is a required header."
            : $"{arg.Name} is a required query string parameter.";

        if (value is null)
        {
            return arg.IsNullable switch
            {
                // if its nullable and has no value, return no errors
                true => [],

                // if its not nullable and has no value, return an error
                false => [new ValidationFailure(arg.Name, message)],
            };
        }

        // if the parameter is not a string and the value is an empty string, return an error
        if (arg.ParameterType != typeof(string) && arg.UnderlyingType != typeof(string) && value is "")
        {
            return [new ValidationFailure(arg.Name, message)];
        }
        
        var castingType = arg.UnderlyingType ?? arg.ParameterType;
        var didCastValue = Utils.TryCastValue(value, castingType, out var castValue, out _);

        if (!didCastValue || castValue is null)
        {
            var castErrors = new List<ValidationFailure>
            {
                new(arg.Name, $"Could not cast {arg.Name} to {castingType.Name}."),
            };
            return castErrors;
        }
        
        var validationContext = new ValidationContext(castValue, context.RequestServices, items: null);

        var errors = arg.ValidationAttributes
            .Where(x => x.GetValidationResult(castValue, validationContext) is not null)
            .Select(x => new ValidationFailure(arg.Name, x.FormatErrorMessage(arg.Name)))
            .ToList();

        return errors;
    }
}
