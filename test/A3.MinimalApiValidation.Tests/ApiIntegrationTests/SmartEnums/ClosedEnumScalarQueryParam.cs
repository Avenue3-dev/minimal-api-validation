namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.SmartEnums;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class ClosedEnumScalarQueryParam : TestBase
{
    public ClosedEnumScalarQueryParam(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, ([FromQuery] Animal? animal = null) => TypedResults.Ok(animal?.Value));
    }

    [Theory]
    [InlineData("cat")]
    [InlineData("dog")]
    [InlineData("bird")]
    public async Task returns_ok_for_a_defined_value(string animal)
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?animal={animal}");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task returns_ok_when_absent()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task returns_bad_request_for_an_unknown_value()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?animal=fish");

        // Assert
        await response.EnsureErrorFor("animal");
    }
}
