namespace A3.MinimalApiValidation.Internal.Middleware;

using System.ComponentModel.DataAnnotations;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

internal static class HeaderOrQuery
{
    public static IEnumerable<ValidationFailure> Handle(ParameterAttributeInfo arg, HttpContext context)
    {
        var castingType = arg.UnderlyingType ?? arg.ParameterType;

        // collection-typed parameters (e.g. string[], int[], List<T>) are read and validated
        // per element, with any validation attributes applied to the materialised collection.
        if (Utils.IsCollectionType(castingType, out var elementType))
        {
            return HandleCollection(arg, context, elementType);
        }

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

    private static IEnumerable<ValidationFailure> HandleCollection(
        ParameterAttributeInfo arg,
        HttpContext context,
        Type elementType)
    {
        var values = arg.IsHeader
            ? context.Request.Headers[arg.Name]
            : context.Request.Query[arg.Name];

        var message = arg.IsHeader
            ? $"{arg.Name} is a required header."
            : $"{arg.Name} is a required query string parameter.";

        // an absent collection is treated the same as an absent scalar: allowed when nullable.
        if (values.Count == 0)
        {
            return arg.IsNullable
                ? []
                : [new ValidationFailure(arg.Name, message)];
        }

        var errors = new List<ValidationFailure>();
        var parsed = new List<object?>();

        for (var i = 0; i < values.Count; i++)
        {
            if (!Utils.TryCastValue(values[i], elementType, out var castValue, out _) || castValue is null)
            {
                errors.Add(new ValidationFailure(
                    $"{arg.Name}[{i}]",
                    $"Could not cast {arg.Name}[{i}] to {elementType.Name}."));
                continue;
            }

            parsed.Add(castValue);
        }

        // materialise into a strongly-typed array so that collection-level validation attributes
        // (e.g. [MinLength], [MaxLength], [Length], [Required]) see the real element type and count.
        var collection = Array.CreateInstance(elementType, parsed.Count);
        for (var i = 0; i < parsed.Count; i++)
        {
            collection.SetValue(parsed[i], i);
        }

        var validationContext = new ValidationContext(collection, context.RequestServices, items: null);

        errors.AddRange(arg.ValidationAttributes
            .Where(x => x.GetValidationResult(collection, validationContext) is not null)
            .Select(x => new ValidationFailure(arg.Name, x.FormatErrorMessage(arg.Name))));

        return errors;
    }
}
