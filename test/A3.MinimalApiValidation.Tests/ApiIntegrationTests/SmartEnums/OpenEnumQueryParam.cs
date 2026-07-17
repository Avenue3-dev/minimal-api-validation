namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.SmartEnums;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

// Open enumerations accept any non-empty value, so validation never rejects an unknown value.
public class OpenEnumQueryParam : TestBase
{
    public OpenEnumQueryParam(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, ([FromQuery] Status? status = null) => TypedResults.Ok(status?.Value));
    }

    [Theory]
    [InlineData("active")]
    [InlineData("archived")]
    [InlineData("something-unknown")]
    public async Task returns_ok_for_any_value(string status)
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?status={status}");

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
}
