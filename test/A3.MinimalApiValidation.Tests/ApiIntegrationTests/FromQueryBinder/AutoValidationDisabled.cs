namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.FromQueryBinder;

using A3.MinimalApiValidation.Binders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class AutoValidationDisabled : TestBase
{
    public AutoValidationDisabled(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void ConfigureOptions(EndpointValidatorOptions options)
    {
        options.PreferExplicitRequestBodyValidation = true;
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, (FromQuery<TestRecord> query) => TypedResults.Ok(query));
    }

    [Theory]
    [InlineData("John", 0)]
    [InlineData("John", 101)]
    [InlineData(null, 30)]
    [InlineData("", 30)]
    [InlineData(" ", 30)]
    [InlineData("John", 30)]
    public async Task returns_ok_for_any_values_when_auto_validation_is_disabled(string? name, int age)
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?name={name}&age={age}");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
