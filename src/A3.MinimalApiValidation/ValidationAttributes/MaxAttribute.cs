namespace A3.MinimalApiValidation.ValidationAttributes;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// A validation attribute that specifies the inclusive maximum value allowed for the data field.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class MaxAttribute : ValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MaxAttribute"/> with the specified <see cref="int"/> value.
    /// <para>
    /// When validation is performed, the value of the attributed property must be a number and less than or equal to the specified value.
    /// </para>
    /// </summary>
    /// <param name="maxValue">The inclusive maximum value.</param>
    /// <example>5</example>
    public MaxAttribute(int maxValue)
    {
        MaxValue = maxValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MaxAttribute"/> with the specified value or property name.
    /// <para>
    /// The value provided can by either a parsable ISO DateTime string or a number.
    /// </para>
    /// <para>
    /// When validation is performed, the value of the attributed property must be less than or equal to the specified value.
    /// </para>
    /// </summary>
    /// <param name="maxValue">The inclusive maximum value.</param>
    /// <example>"5" | "2021-02-05"</example>
    public MaxAttribute(string maxValue)
    {
        if (DateTime.TryParse(maxValue, out var dateValue))
        {
            MaxDateValue = dateValue;
            IsDateValue = true;
        }
        else if (int.TryParse(maxValue, out var intValue))
        {
            MaxValue = intValue;
        }
        else
        {
            throw new ArgumentException(
                $"The value provided must be a parsable ISO DateTime string or a number. The value provided was: {maxValue}",
                nameof(maxValue));
        }
    }

    private int MaxValue { get; }
    
    private DateTime MaxDateValue { get; }
    
    private bool IsDateValue { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value)
    {
        return value is int i && i <= MaxValue
            || value is long l && l <= MaxValue
            || value is float f && f <= MaxValue
            || value is double d && d <= MaxValue
            || value is decimal dec && dec <= MaxValue
            || value is DateTime dt && IsDateValue && dt <= MaxDateValue;
    }

    /// <inheritdoc />
    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be less than or equal to {(IsDateValue ? MaxDateValue : MaxValue)}";
    }
}
