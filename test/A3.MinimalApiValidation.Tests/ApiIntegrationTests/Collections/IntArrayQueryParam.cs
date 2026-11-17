namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.Collections;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class IntArrayQueryParam : TestBase
{
    public IntArrayQueryParam(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, ([FromQuery] int[]? number = null) => TypedResults.Ok(number));
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
    public async Task returns_ok_for_repeated_valid_values()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?number=1&number=2&number=3");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task returns_bad_request_when_an_element_is_not_an_int()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?number=1&number=abc");

        // Assert
        await response.EnsureErrorFor("number[1]");
    }
}
