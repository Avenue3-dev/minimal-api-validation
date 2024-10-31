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
            PreferExplicitRequestBodyValidation = false,
        };

        // Act
        var actual = EndpointValidatorOptions.Default;

        // Assert
        Assert.Equivalent(expected, actual);
    }
}
