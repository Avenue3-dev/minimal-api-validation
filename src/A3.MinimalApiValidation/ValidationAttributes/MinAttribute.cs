namespace A3.MinimalApiValidation.ValidationAttributes;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// A validation attribute that specifies the inclusive minimum value allowed for the data field.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class MinAttribute : ValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MinAttribute"/> with the specified <see cref="int"/> value.
    /// <para>
    /// When validation is performed, the value of the attributed property must be a number and greater than or equal to the specified value.
    /// </para>
    /// </summary>
    /// <param name="minValue">The inclusive minimum value.</param>
    /// <example>5</example>
    public MinAttribute(int minValue)
    {
        MinValue = minValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinAttribute"/> with the specified value or property name.
    /// <para>
    /// The value provided can by either a parsable ISO DateTime string or a number.
    /// </para>
    /// <para>
    /// When validation is performed, the value of the attributed property must be greater than or equal to the specified value.
    /// </para>
    /// </summary>
    /// <param name="minValue">The inclusive minimum value.</param>
    /// <example>"5" | "2021-02-05"</example>
    public MinAttribute(string minValue)
    {
        if (DateTime.TryParse(minValue, out var dateValue))
        {
            MinDateValue = dateValue;
            IsDateValue = true;
        }
        else if (int.TryParse(minValue, out var intValue))
        {
            MinValue = intValue;
        }
        else
        {
            throw new ArgumentException(
                $"The value provided must be a parsable ISO DateTime string or a number. The value provided was: {minValue}",
                nameof(minValue));
        }
    }

    private int MinValue { get; }
    
    private DateTime MinDateValue { get; }
    
    private bool IsDateValue { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value)
    {
        return value is int i && i >= MinValue
            || value is long l && l >= MinValue
            || value is float f && f >= MinValue
            || value is double d && d >= MinValue
            || value is decimal dec && dec >= MinValue
            || value is DateTime dt && IsDateValue && dt >= MinDateValue;
    }

    /// <inheritdoc />
    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be greater than or equal to {(IsDateValue ? MinDateValue : MinValue)}";
    }
}
