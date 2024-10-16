namespace EndpointValidator.Internal;

using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

internal static class HeaderOrQuery
{
    public static List<ValidationFailure> Handle(ParameterAttributeInfo arg, HttpContext context)
    {
        var value = arg.IsHeader
            ? context.Request.Headers[arg.Name].FirstOrDefault()
            : context.Request.Query[arg.Name].FirstOrDefault();
        
        var hasValue = value is not null;

        if (arg.IsNullable && !hasValue)
        {
            return [];
        }

        if (!arg.IsNullable && !hasValue)
        {
            var message = arg.IsHeader
                ? $"{arg.Name} is a required header."
                : $"{arg.Name} is a required query string parameter.";

            return [new ValidationFailure(arg.Name, message)];
        }

        var castValue = hasValue
            ? arg.UnderlyingType is not null
                ? Convert.ChangeType(value, arg.UnderlyingType)
                : Convert.ChangeType(value, arg.ParameterType)
            : null;

        var errors = arg.ValidationAttributes
            .Where(x => !x.IsValid(castValue))
            .Select(x => new ValidationFailure(arg.Name, x.FormatErrorMessage(arg.Name)))
            .ToList();

        return errors;
    }
}
