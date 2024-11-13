namespace A3.MinimalApiValidation.Tests;

public class EndpointValidatorOptionsTests
{
    [Fact]
    public void Default_has_correct_values()
    {
        // Arrange
        var expected = new EndpointValidatorOptions
        {
            FallbackToDataAnnotations = false,
            JsonSerializerOptions = null,
            PreferExplicitRequestModelValidation = false,
            ValidateOnlyHeader = "x-validate-only",
        };

        // Act
        var actual = EndpointValidatorOptions.Default;

        // Assert
        Assert.Equivalent(expected, actual);
    }
}
