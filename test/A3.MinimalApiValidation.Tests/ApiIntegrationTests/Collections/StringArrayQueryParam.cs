namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.Collections;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class StringArrayQueryParam : TestBase
{
    public StringArrayQueryParam(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, ([FromQuery] string[]? tag = null) => TypedResults.Ok(tag));
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
    public async Task returns_ok_for_a_single_value()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?tag=a");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task returns_ok_for_repeated_keys()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?tag=a&tag=b");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
