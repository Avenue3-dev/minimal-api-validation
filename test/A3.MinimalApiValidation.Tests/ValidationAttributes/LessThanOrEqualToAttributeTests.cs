namespace A3.MinimalApiValidation.Tests.ValidationAttributes;

using System.ComponentModel.DataAnnotations;
using A3.MinimalApiValidation.ValidationAttributes;

public class LessThanOrEqualToAttributeTests
{
    #region Valid
    
    [Theory]
    [InlineData(3, 3)]
    [InlineData(3, 4)]
    [InlineData(3, 5)]
    [InlineData(3, 6)]
    public void GetValidationResult_returns_null_when_int_value_is_less_than_or_equal_to_max(int value, int max)
    {
        // Arrange
        var sut = new LessThanOrEqualToAttribute(max);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.Null(result);
    }
    
    [Theory]
    [InlineData(3L, 3)]
    [InlineData(3L, 4)]
    [InlineData(3L, 5)]
    [InlineData(3L, 6)]
    public void GetValidationResult_returns_null_when_long_value_is_less_than_or_equal_to_max(long value, int max)
    {
        // Arrange
        var sut = new LessThanOrEqualToAttribute(max);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.Null(result);
    }
    
    [Theory]
    [InlineData(3F, 3)]
    [InlineData(3F, 4)]
    [InlineData(3F, 5)]
    [InlineData(3F, 6)]
    public void GetValidationResult_returns_null_when_float_value_is_less_than_or_equal_to_max(float value, int max)
    {
        // Arrange
        var sut = new LessThanOrEqualToAttribute(max);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.Null(result);
    }
    
    [Theory]
    [InlineData(3D, 3)]
    [InlineData(3D, 4)]
    [InlineData(3D, 5)]
    [InlineData(3D, 6)]
    public void GetValidationResult_returns_null_when_double_value_is_less_than_or_equal_to_max(double value, int max)
    {
        // Arrange
        var sut = new LessThanOrEqualToAttribute(max);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.Null(result);
    }
    
    [Theory]
    [InlineData(3, 3)]
    [InlineData(3, 4)]
    [InlineData(3, 5)]
    [InlineData(3, 6)]
    public void GetValidationResult_returns_null_when_decimal_value_is_less_than_or_equal_to_max(decimal value, int max)
    {
        // Arrange
        var sut = new LessThanOrEqualToAttribute(max);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.Null(result);
    }
    
    [Theory]
    [InlineData(3, "3")]
    [InlineData(3, "4")]
    [InlineData(3, "5")]
    [InlineData(3, "6")]
    public void GetValidationResult_returns_null_when_int_value_is_less_than_or_equal_to_max_as_string(int value, string max)
    {
        // Arrange
        var sut = new LessThanOrEqualToAttribute(max);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.Null(result);
    }
    
    [Theory]
    [InlineData("2022-02-22")]
    [InlineData("2022-02-21")]
    [InlineData("2022-02-20")]
    [InlineData("2022-02-19")]
    public void GetValidationResult_returns_null_when_date_time_value_is_less_than_or_equal_to_max(string value)
    {
        // Arrange
        var sut = new LessThanOrEqualToAttribute("2022-02-22");
        var context = new ValidationContext(DateTime.Parse(value), Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(DateTime.Parse(value), context);
        
        // Assert
        Assert.Null(result);
    }
    
    #endregion
    
    #region Invalid
    
    [Theory]
    [InlineData(4, 3)]
    [InlineData(5, 3)]
    [InlineData(6, 3)]
    public void GetValidationResult_returns_errors_when_int_value_is_greater_than_max(int value, int max)
    {
        // Arrange
        var sut = new LessThanOrEqualToAttribute(max);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.NotNull(result);
    }
    
    [Theory]
    [InlineData(4L, 3)]
    [InlineData(5L, 3)]
    [InlineData(6L, 3)]
    public void GetValidationResult_returns_errors_when_long_value_is_greater_than_max(long value, int max)
    {
        // Arrange
        var sut = new LessThanOrEqualToAttribute(max);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.NotNull(result);
    }
    
    [Theory]
    [InlineData(4F, 3)]
    [InlineData(5F, 3)]
    [InlineData(6F, 3)]
    public void GetValidationResult_returns_errors_when_float_value_is_greater_than_max(float value, int max)
    {
        // Arrange
        var sut = new LessThanOrEqualToAttribute(max);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.NotNull(result);
    }
    
    [Theory]
    [InlineData(4D, 3)]
    [InlineData(5D, 3)]
    [InlineData(6D, 3)]
    public void GetValidationResult_returns_errors_when_double_value_is_greater_than_max(double value, int max)
    {
        // Arrange
        var sut = new LessThanOrEqualToAttribute(max);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.NotNull(result);
    }
    
    [Theory]
    [InlineData(4, 3)]
    [InlineData(5, 3)]
    [InlineData(6, 3)]
    public void GetValidationResult_returns_errors_when_decimal_value_is_greater_than_max(decimal value, int max)
    {
        // Arrange
        var sut = new LessThanOrEqualToAttribute(max);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.NotNull(result);
    }
    
    [Theory]
    [InlineData(4, "3")]
    [InlineData(5, "3")]
    [InlineData(6, "3")]
    public void GetValidationResult_returns_errors_when_int_value_is_greater_than_max_as_string(int value, string max)
    {
        // Arrange
        var sut = new LessThanOrEqualToAttribute(max);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.NotNull(result);
    }
    
    [Theory]
    [InlineData("2022-02-23")]
    [InlineData("2022-02-24")]
    [InlineData("2022-02-25")]
    public void GetValidationResult_returns_errors_when_date_time_value_is_greater_than_max(string value)
    {
        // Arrange
        var sut = new LessThanOrEqualToAttribute("2022-02-22");
        var context = new ValidationContext(DateTime.Parse(value), Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(DateTime.Parse(value), context);
        
        // Assert
        Assert.NotNull(result);
    }
    
    #endregion
}
