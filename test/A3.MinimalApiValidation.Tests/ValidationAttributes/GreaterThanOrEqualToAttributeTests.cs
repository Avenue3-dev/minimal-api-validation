namespace A3.MinimalApiValidation.Tests.ValidationAttributes;

using System.ComponentModel.DataAnnotations;
using A3.MinimalApiValidation.ValidationAttributes;

public class GreaterThanOrEqualToAttributeTests
{
    #region Valid
    
    [Theory]
    [InlineData(3, 3)]
    [InlineData(4, 3)]
    [InlineData(5, 3)]
    [InlineData(6, 3)]
    public void GetValidationResult_returns_null_when_int_value_is_greater_than_or_equal_to_min(int value, int min)
    {
        // Arrange
        var sut = new GreaterThanOrEqualToAttribute(min);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.Null(result);
    }
    
    [Theory]
    [InlineData(3L, 3)]
    [InlineData(4L, 3)]
    [InlineData(5L, 3)]
    [InlineData(6L, 3)]
    public void GetValidationResult_returns_null_when_long_value_is_greater_than_or_equal_to_min(long value, int min)
    {
        // Arrange
        var sut = new GreaterThanOrEqualToAttribute(min);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.Null(result);
    }
    
    [Theory]
    [InlineData(3F, 3)]
    [InlineData(4F, 3)]
    [InlineData(5F, 3)]
    [InlineData(6F, 3)]
    public void GetValidationResult_returns_null_when_float_value_is_greater_than_or_equal_to_min(float value, int min)
    {
        // Arrange
        var sut = new GreaterThanOrEqualToAttribute(min);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.Null(result);
    }
    
    [Theory]
    [InlineData(3D, 3)]
    [InlineData(4D, 3)]
    [InlineData(5D, 3)]
    [InlineData(6D, 3)]
    public void GetValidationResult_returns_null_when_double_value_is_greater_than_or_equal_to_min(double value, int min)
    {
        // Arrange
        var sut = new GreaterThanOrEqualToAttribute(min);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.Null(result);
    }
    
    [Theory]
    [InlineData(3, "3")]
    [InlineData(4, "3")]
    [InlineData(5, "3")]
    [InlineData(6, "3")]
    public void GetValidationResult_returns_null_when_int_value_is_greater_than_or_equal_to_min_as_string(int value, string min)
    {
        // Arrange
        var sut = new GreaterThanOrEqualToAttribute(min);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.Null(result);
    }
    
    [Theory]
    [InlineData("2022-02-22")]
    [InlineData("2022-02-23")]
    [InlineData("2022-02-24")]
    public void GetValidationResult_returns_null_when_date_time_value_is_greater_than_or_equal_to_min(string value)
    {
        // Arrange
        var sut = new GreaterThanOrEqualToAttribute("2022-02-22");
        var context = new ValidationContext(DateTime.Parse(value), Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(DateTime.Parse(value), context);
        
        // Assert
        Assert.Null(result);
    }

    #endregion
    
    #region Invalid
    
    [Theory]
    [InlineData(2, 3)]
    [InlineData(1, 3)]
    [InlineData(0, 3)]
    public void GetValidationResult_returns_errors_when_int_value_is_less_than_min(int value, int min)
    {
        // Arrange
        var sut = new GreaterThanOrEqualToAttribute(min);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.NotNull(result);
    }
    
    [Theory]
    [InlineData(2L, 3)]
    [InlineData(1L, 3)]
    [InlineData(0L, 3)]
    public void GetValidationResult_returns_errors_when_long_value_is_less_than_min(long value, int min)
    {
        // Arrange
        var sut = new GreaterThanOrEqualToAttribute(min);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.NotNull(result);
    }
    
    [Theory]
    [InlineData(2F, 3)]
    [InlineData(1F, 3)]
    [InlineData(0F, 3)]
    public void GetValidationResult_returns_errors_when_float_value_is_less_than_min(float value, int min)
    {
        // Arrange
        var sut = new GreaterThanOrEqualToAttribute(min);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.NotNull(result);
    }
    
    [Theory]
    [InlineData(2D, 3)]
    [InlineData(1D, 3)]
    [InlineData(0D, 3)]
    public void GetValidationResult_returns_errors_when_double_value_is_less_than_min(double value, int min)
    {
        // Arrange
        var sut = new GreaterThanOrEqualToAttribute(min);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.NotNull(result);
    }
    
    [Theory]
    [InlineData(2, "3")]
    [InlineData(1, "3")]
    [InlineData(0, "3")]
    public void GetValidationResult_returns_errors_when_int_value_is_less_than_min_as_string(int value, string min)
    {
        // Arrange
        var sut = new GreaterThanOrEqualToAttribute(min);
        var context = new ValidationContext(value, Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(value, context);
        
        // Assert
        Assert.NotNull(result);
    }
    
    [Theory]
    [InlineData("2022-02-21")]
    [InlineData("2022-02-20")]
    [InlineData("2022-02-19")]
    public void GetValidationResult_returns_null_when_date_time_value_is_less_than_min(string value)
    {
        // Arrange
        var sut = new GreaterThanOrEqualToAttribute("2022-02-22");
        var context = new ValidationContext(DateTime.Parse(value), Substitute.For<IServiceProvider>(), items: null);
        
        // Act
        var result = sut.GetValidationResult(DateTime.Parse(value), context);
        
        // Assert
        Assert.NotNull(result);
    }
    
    #endregion
}
