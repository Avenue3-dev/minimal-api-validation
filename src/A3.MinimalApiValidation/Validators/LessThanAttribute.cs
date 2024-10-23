namespace A3.MinimalApiValidation.Validators;

using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Specifies that a data field value must be less than the provided value or property. 
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class LessThanAttribute : ValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LessThanAttribute"/> with the specified <see cref="int"/> value.
    /// <para>
    /// When validation is performed, the value of the attributed property must be a number and less than the specified value.
    /// </para>
    /// </summary>
    /// <param name="value">The exclusive maximum value.</param>
    /// <example>5</example>
    public LessThanAttribute(int value)
    {
        MaxValue = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LessThanAttribute"/> with the specified value or property name.
    /// <para>
    /// The value provided can by either a parsable ISO DateTime string, a number, or the name of another header, query string, or model property.
    /// </para>
    /// <para>
    /// When validation is performed, the value of the attributed property must be less than the specified value, or value of the property with the specified name.
    /// </para>
    /// </summary>
    /// <param name="valueOrPropertyName">The exclusive maximum value, or name of a property to compare to.</param>
    /// <example>"5" | "2021-02-05" | "SomeOtherProperty" | nameof(SomeOtherProperty)</example>
    public LessThanAttribute(string valueOrPropertyName)
    {
        if (DateTime.TryParse(valueOrPropertyName, out var dateValue))
        {
            MaxDateValue = dateValue;
        }
        else if (int.TryParse(valueOrPropertyName, out var intValue))
        {
            MaxValue = intValue;
        }
        else
        {
            OtherPropertyName = valueOrPropertyName;
        }
    }

    private int MaxValue { get; }

    private DateTime MaxDateValue { get; }

    private string? OtherPropertyName { get; }

    /// <inheritdoc />
    public override bool RequiresValidationContext => true;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // if another property is provided, compare the two
        // precedence: body, query, header
        if (!string.IsNullOrWhiteSpace(OtherPropertyName))
        {
            var httpContext = validationContext.GetService<IHttpContextAccessor>()?.HttpContext;
            var otherProperty = validationContext.ObjectType.GetRuntimeProperty(OtherPropertyName);
            
            var otherPropertyValue = otherProperty is not null
                ? otherProperty.GetValue(validationContext.ObjectInstance, null)
                : httpContext?.Request.Query[OtherPropertyName].FirstOrDefault()
                ?? httpContext?.Request.Headers[OtherPropertyName].FirstOrDefault();

            switch (value)
            {
                case int i when int.TryParse(otherPropertyValue?.ToString(), out var j) && i < j:
                case long l when long.TryParse(otherPropertyValue?.ToString(), out var m) && l < m:
                case float f when float.TryParse(otherPropertyValue?.ToString(), out var g) && f < g:
                case double d when double.TryParse(otherPropertyValue?.ToString(), out var e) && d < e:
                case decimal dec when decimal.TryParse(otherPropertyValue?.ToString(), out var dec2) && dec < dec2:
                case DateTime dt when DateTime.TryParse(otherPropertyValue?.ToString(), out var dt2) && dt < dt2:
                    return ValidationResult.Success;
                default:
                    return validationContext.Error($"must be less than {OtherPropertyName}");
            }
        }

        // if a date value is provided, compare the two
        if (MaxDateValue != default)
        {
            if (value is not DateTime dt)
            {
                return validationContext.Error("must be a date.");
            }

            return dt < MaxDateValue
                ? ValidationResult.Success
                : validationContext.Error($"must be less than {MaxDateValue}");
        }

        // if the value is not a number, return an error
        if (value is not int
            && value is not long
            && value is not float
            && value is not double
            && value is not decimal)
        {
            return validationContext.Error("must be a number.");
        }

        // otherwise, compare the two numbers
        switch (value)
        {
            case int i when i < MaxValue:
            case long l when l < MaxValue:
            case float f when f < MaxValue:
            case double d when d < MaxValue:
            case decimal dec when dec < MaxValue:
                return ValidationResult.Success;
            default:
                return validationContext.Error($"must be less than {MaxValue}");
        }
    }
}
