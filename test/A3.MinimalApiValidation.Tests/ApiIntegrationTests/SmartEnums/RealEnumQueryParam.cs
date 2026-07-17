namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.SmartEnums;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

// A regular C# enum is validated via the Enum.TryParse convention.
public class RealEnumQueryParam : TestBase
{
    public RealEnumQueryParam(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, ([FromQuery] DayOfWeek? day = null) => TypedResults.Ok(day));
    }

    [Fact]
    public async Task returns_ok_for_a_valid_value()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?day=Monday");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task returns_bad_request_for_an_invalid_value()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?day=NotADay");

        // Assert
        await response.EnsureErrorFor("day");
    }
}
