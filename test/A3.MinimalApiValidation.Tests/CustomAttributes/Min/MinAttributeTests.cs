namespace A3.MinimalApiValidation.Tests.CustomAttributes.Min;

using A3.MinimalApiValidation.Validators;

public class MinAttributeTests
{
    #region Valid
    
    [Theory]
    [InlineData(3, 3)]
    [InlineData(4, 3)]
    [InlineData(5, 3)]
    public void IsValid_returns_true_when_int_value_is_greater_than_or_equal_to_min(int value, int min)
    {
        // Arrange
        var model = new MinAttribute(min);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.True(result);
    }
    
    [Theory]
    [InlineData(3L, 3)]
    [InlineData(4L, 3)]
    [InlineData(5L, 3)]
    public void IsValid_returns_true_when_long_value_is_greater_than_or_equal_to_min(long value, int min)
    {
        // Arrange
        var model = new MinAttribute(min);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.True(result);
    }
    
    [Theory]
    [InlineData(3F, 3)]
    [InlineData(4F, 3)]
    [InlineData(5F, 3)]
    public void IsValid_returns_true_when_float_value_is_greater_than_or_equal_to_min(float value, int min)
    {
        // Arrange
        var model = new MinAttribute(min);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.True(result);
    }
    
    [Theory]
    [InlineData(3D, 3)]
    [InlineData(4D, 3)]
    [InlineData(5D, 3)]
    public void IsValid_returns_true_when_double_value_is_greater_than_or_equal_to_min(double value, int min)
    {
        // Arrange
        var model = new MinAttribute(min);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.True(result);
    }
    
    [Theory]
    [InlineData(3, 3)]
    [InlineData(4, 3)]
    [InlineData(5, 3)]
    public void IsValid_returns_true_when_decimal_value_is_greater_than_or_equal_to_min(decimal value, int min)
    {
        // Arrange
        var model = new MinAttribute(min);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.True(result);
    }
    
    [Theory]
    [InlineData(3, "3")]
    [InlineData(4, "3")]
    [InlineData(5, "3")]
    public void IsValid_returns_true_when_int_value_is_greater_than_or_equal_to_min_as_string(int value, string min)
    {
        // Arrange
        var model = new MinAttribute(min);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.True(result);
    }
    
    [Theory]
    [InlineData("2022-02-22")]
    [InlineData("2022-02-23")]
    [InlineData("2022-02-24")]
    public void IsValid_returns_true_when_date_time_value_is_greater_than_or_equal_to_min(string value)
    {
        // Arrange
        var model = new MinAttribute("2022-02-22");
        
        // Act
        var result = model.IsValid(DateTime.Parse(value));
        
        // Assert
        Assert.True(result);
    }
    
    #endregion
    
    #region Invalid
    
    [Theory]
    [InlineData(2, 3)]
    [InlineData(1, 3)]
    [InlineData(0, 3)]
    public void IsValid_returns_false_when_int_value_is_less_than_min(int value, int min)
    {
        // Arrange
        var model = new MinAttribute(min);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData(2L, 3)]
    [InlineData(1L, 3)]
    [InlineData(0L, 3)]
    public void IsValid_returns_false_when_long_value_is_less_than_min(long value, int min)
    {
        // Arrange
        var model = new MinAttribute(min);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData(2F, 3)]
    [InlineData(1F, 3)]
    [InlineData(0F, 3)]
    public void IsValid_returns_false_when_float_value_is_less_than_min(float value, int min)
    {
        // Arrange
        var model = new MinAttribute(min);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData(2D, 3)]
    [InlineData(1D, 3)]
    [InlineData(0D, 3)]
    public void IsValid_returns_false_when_double_value_is_less_than_min(double value, int min)
    {
        // Arrange
        var model = new MinAttribute(min);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData(2, 3)]
    [InlineData(1, 3)]
    [InlineData(0, 3)]
    public void IsValid_returns_false_when_decimal_value_is_less_than_min(decimal value, int min)
    {
        // Arrange
        var model = new MinAttribute(min);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData(2, "3")]
    [InlineData(1, "3")]
    [InlineData(0, "3")]
    public void IsValid_returns_false_when_int_value_is_less_than_min_as_string(int value, string min)
    {
        // Arrange
        var model = new MinAttribute(min);
        
        // Act
        var result = model.IsValid(value);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData("2022-02-21")]
    [InlineData("2022-02-20")]
    [InlineData("2022-02-19")]
    public void IsValid_returns_false_when_date_time_value_is_less_than_min(string value)
    {
        // Arrange
        var model = new MinAttribute("2022-02-22");
        
        // Act
        var result = model.IsValid(DateTime.Parse(value));
        
        // Assert
        Assert.False(result);
    }
    
    #endregion
}
