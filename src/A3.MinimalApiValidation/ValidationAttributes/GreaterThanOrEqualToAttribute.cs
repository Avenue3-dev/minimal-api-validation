namespace A3.MinimalApiValidation.ValidationAttributes;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Specifies that a data field value must be greater than or equal to the provided value or property. 
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class GreaterThanOrEqualToAttribute : ValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanOrEqualToAttribute"/> with the specified <see cref="int"/> value.
    /// <para>
    /// When validation is performed, the value of the attributed property must be a number and greater than or equal to the specified value.
    /// </para>
    /// </summary>
    /// <param name="value">The inclusive minimum value.</param>
    /// <example>5</example>
    public GreaterThanOrEqualToAttribute(int value)
    {
        MinValue = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanOrEqualToAttribute"/> with the specified value or property name.
    /// <para>
    /// The value provided can by either a parsable ISO DateTime string, a number, or the name of another header, query string, or model property.
    /// </para>
    /// <para>
    /// When validation is performed, the value of the attributed property must be greater than or equal to the specified value, or value of the property with the specified name.
    /// </para>
    /// </summary>
    /// <param name="valueOrPropertyName">The inclusive minimum value, or name of a property to compare to.</param>
    /// <example>"5" | "2021-02-05" | "SomeOtherProperty" | nameof(SomeOtherProperty)</example>
    public GreaterThanOrEqualToAttribute(string valueOrPropertyName)
    {
        if (DateTime.TryParse(valueOrPropertyName, out var dateValue))
        {
            MinDateValue = dateValue;
        }
        else if (int.TryParse(valueOrPropertyName, out var intValue))
        {
            MinValue = intValue;
        }
        else
        {
            OtherPropertyName = valueOrPropertyName;
        }
    }

    private int MinValue { get; }

    private DateTime MinDateValue { get; }

    private string? OtherPropertyName { get; }

    /// <inheritdoc />
    public override bool RequiresValidationContext => true;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // if another property is provided, compare the two
        if (!string.IsNullOrWhiteSpace(OtherPropertyName))
        {
            // try get other property from current object
            var otherPropertyValue = validationContext.GetOtherPropertyValue(OtherPropertyName);

            switch (value)
            {
                case int i when int.TryParse(otherPropertyValue?.ToString(), out var j) && i >= j:
                case long l when long.TryParse(otherPropertyValue?.ToString(), out var m) && l >= m:
                case float f when float.TryParse(otherPropertyValue?.ToString(), out var g) && f >= g:
                case double d when double.TryParse(otherPropertyValue?.ToString(), out var e) && d >= e:
                case decimal dec when decimal.TryParse(otherPropertyValue?.ToString(), out var dec2) && dec >= dec2:
                case DateTime dt when DateTime.TryParse(otherPropertyValue?.ToString(), out var dt2) && dt >= dt2:
                    return ValidationResult.Success;
                default:
                    return validationContext.Error($"must be greater than or equal to {OtherPropertyName}");
            }
        }

        // if a date value is provided, compare the two
        if (MinDateValue != default)
        {
            if (value is not DateTime dt)
            {
                return validationContext.Error("must be a date.");
            }

            return dt >= MinDateValue
                ? ValidationResult.Success
                : validationContext.Error($"must be greater than or equal to {MinDateValue}");
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
            case int i when i >= MinValue:
            case long l when l >= MinValue:
            case float f when f >= MinValue:
            case double d when d >= MinValue:
            case decimal dec when dec >= MinValue:
                return ValidationResult.Success;
            default:
                return validationContext.Error($"must be greater than or equal to {MinValue}");
        }
    }
}
