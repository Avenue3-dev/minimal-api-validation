namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.SmartEnums;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class OpenEnumArrayQueryParam : TestBase
{
    public OpenEnumArrayQueryParam(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, ([FromQuery] Status[]? status = null) => TypedResults.Ok(status));
    }

    [Fact]
    public async Task returns_ok_for_repeated_keys_including_unknown_values()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?status=active&status=something-unknown");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
