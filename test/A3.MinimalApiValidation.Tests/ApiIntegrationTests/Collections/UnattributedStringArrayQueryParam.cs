namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.Collections;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

// Regression guard: parameters without an explicit [FromQuery]/[FromHeader] attribute are ignored
// by the middleware, so the consumer's "drop the attribute" workaround keeps working.
public class UnattributedStringArrayQueryParam : TestBase
{
    public UnattributedStringArrayQueryParam(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, (string[]? tag) => TypedResults.Ok(tag));
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
