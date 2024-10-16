namespace EndpointValidator.Internal;

using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;

internal record ParameterAttributeInfo
{
    public ParameterAttributeInfo(ParameterInfo parameter)
    {
        var attributes = parameter.GetCustomAttributes().ToList();

        var header = attributes.OfType<FromHeaderAttribute>().FirstOrDefault();
        var query = attributes.OfType<FromQueryAttribute>().FirstOrDefault();
        var body = attributes.OfType<FromBodyAttribute>().FirstOrDefault();

        IsBody = body is not null;
        IsQuery = query is not null;
        IsHeader = header is not null;

        if (IsBody && IsQuery || IsBody && IsHeader || IsQuery && IsHeader)
        {
            throw new InvalidOperationException(
                "Parameter can only be one of FromBody, FromQuery, or FromHeader.");
        }

        Name = header?.Name ?? query?.Name ?? parameter.Name
            ?? throw new ArgumentNullException(nameof(parameter), "Parameter name is required but was null.");

        ParameterType = parameter.ParameterType;

        UnderlyingType = Nullable.GetUnderlyingType(parameter.ParameterType);

        IsNullable = attributes.Any(x => x is NullableAttribute) || UnderlyingType is not null;

        ValidationAttributes = attributes
            .Where(x => x.GetType().IsSubclassOf(typeof(ValidationAttribute)))
            .Select(x => (ValidationAttribute) x)
            .ToList();
    }

    public string Name { get; }

    public Type ParameterType { get; }

    public bool IsBody { get; }

    public bool IsQuery { get; }

    public bool IsHeader { get; }

    public bool IsNullable { get; }

    public Type? UnderlyingType { get; }

    public IReadOnlyCollection<ValidationAttribute> ValidationAttributes { get; }
}
