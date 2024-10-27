namespace A3.MinimalApiValidation.Tests.ValidationAttributes;

using A3.MinimalApiValidation.ValidationAttributes;

public class MaxAttributeTests
{
    #region Valid
    
    [Theory]
    [InlineData(-1, 3)]
    [InlineData(0, 3)]
    [InlineData(1, 3)]
    [InlineData(2, 3)]
    [InlineData(3, 3)]
    public void IsValid_returns_true_when_int_value_is_less_than_or_equal_to_max(int value, int max)
    {
        // Arrange
        var model = new MaxAttribute(max);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.True(result);
    }
    
    [Theory]
    [InlineData(-1, 3)]
    [InlineData(0, 3)]
    [InlineData(1, 3)]
    [InlineData(2, 3)]
    [InlineData(3, 3)]
    public void IsValid_returns_true_when_decimal_value_is_less_than_or_equal_to_max(decimal value, int max)
    {
        // Arrange
        var model = new MaxAttribute(max);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.True(result);
    }
    
    [Theory]
    [InlineData(-1L, 3)]
    [InlineData(0L, 3)]
    [InlineData(1L, 3)]
    [InlineData(2L, 3)]
    [InlineData(3L, 3)]
    public void IsValid_returns_true_when_long_value_is_less_than_or_equal_to_max(long value, int max)
    {
        // Arrange
        var model = new MaxAttribute(max);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.True(result);
    }
    
    [Theory]
    [InlineData(-1F, 3)]
    [InlineData(0F, 3)]
    [InlineData(1F, 3)]
    [InlineData(2F, 3)]
    [InlineData(3F, 3)]
    public void IsValid_returns_true_when_float_value_is_less_than_or_equal_to_max(float value, int max)
    {
        // Arrange
        var model = new MaxAttribute(max);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.True(result);
    }
    
    [Theory]
    [InlineData(-1D, 3)]
    [InlineData(0D, 3)]
    [InlineData(1D, 3)]
    [InlineData(2D, 3)]
    [InlineData(3D, 3)]
    public void IsValid_returns_true_when_double_value_is_less_than_or_equal_to_max(double value, int max)
    {
        // Arrange
        var model = new MaxAttribute(max);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.True(result);
    }
    
    [Theory]
    [InlineData(3, "3")]
    [InlineData(2, "3")]
    [InlineData(1, "3")]
    [InlineData(0, "3")]
    public void IsValid_returns_true_when_int_value_is_less_than_or_equal_to_max_as_string(int value, string max)
    {
        // Arrange
        var model = new MaxAttribute(max);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.True(result);
    }
    
    [Theory]
    [InlineData("2022-02-22")]
    [InlineData("2022-02-21")]
    [InlineData("2022-02-20")]
    public void IsValid_returns_true_when_date_time_value_is_less_than_or_equal_to_max(string value)
    {
        // Arrange
        var model = new MaxAttribute("2022-02-22");
        
        // Act
        var result = model.IsValid(DateTime.Parse(value));
        
        // Assert
        Assert.True(result);
    }
    
    #endregion
    
    #region Invalid
    
    [Theory]
    [InlineData(4, 3)]
    [InlineData(5, 3)]
    [InlineData(6, 3)]
    public void IsValid_returns_false_when_int_value_is_greater_than_max(int value, int max)
    {
        // Arrange
        var model = new MaxAttribute(max);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData(4, 3)]
    [InlineData(5, 3)]
    [InlineData(6, 3)]
    public void IsValid_returns_false_when_decimal_value_is_greater_than_max(decimal value, int max)
    {
        // Arrange
        var model = new MaxAttribute(max);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData(4L, 3)]
    [InlineData(5L, 3)]
    [InlineData(6L, 3)]
    public void IsValid_returns_false_when_long_value_is_greater_than_max(long value, int max)
    {
        // Arrange
        var model = new MaxAttribute(max);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData(4F, 3)]
    [InlineData(5F, 3)]
    [InlineData(6F, 3)]
    public void IsValid_returns_false_when_float_value_is_greater_than_max(float value, int max)
    {
        // Arrange
        var model = new MaxAttribute(max);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData(4D, 3)]
    [InlineData(5D, 3)]
    [InlineData(6D, 3)]
    public void IsValid_returns_false_when_double_value_is_greater_than_max(double value, int max)
    {
        // Arrange
        var model = new MaxAttribute(max);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData(4, "3")]
    [InlineData(5, "3")]
    [InlineData(6, "3")]
    public void IsValid_returns_false_when_int_value_is_greater_than_max_as_string(int value, string max)
    {
        // Arrange
        var model = new MaxAttribute(max);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData("2022-02-23")]
    [InlineData("2022-02-24")]
    [InlineData("2022-02-25")]
    public void IsValid_returns_false_when_date_time_value_is_greater_than_max(string value)
    {
        // Arrange
        var model = new MaxAttribute("2022-02-22");
        
        // Act
        var result = model.IsValid(DateTime.Parse(value));
        
        // Assert
        Assert.False(result);
    }
    
    #endregion
}
