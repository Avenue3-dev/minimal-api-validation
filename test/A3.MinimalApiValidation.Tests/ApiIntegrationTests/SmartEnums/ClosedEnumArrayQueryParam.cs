namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.SmartEnums;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

// The "1-3 items, each one of cat/dog/bird" scenario, end to end:
//   - cardinality via [MinLength]/[MaxLength] on the collection
//   - the closed set via the element type failing to parse unknown values
public class ClosedEnumArrayQueryParam : TestBase
{
    public ClosedEnumArrayQueryParam(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, ([FromQuery, MinLength(1), MaxLength(3)] Animal[]? animal = null)
            => TypedResults.Ok(animal));
    }

    [Fact]
    public async Task returns_ok_for_defined_values_within_the_limit()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?animal=cat&animal=dog");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task returns_bad_request_when_an_element_is_unknown()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?animal=cat&animal=fish");

        // Assert
        await response.EnsureErrorFor("animal[1]");
    }

    [Fact]
    public async Task returns_bad_request_when_too_many_items()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?animal=cat&animal=dog&animal=bird&animal=cat");

        // Assert
        await response.EnsureErrorFor("animal");
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
