namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.Collections;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

// Validation attributes on a collection parameter are applied to the collection itself
// (matching System.ComponentModel.DataAnnotations semantics), so [MaxLength] limits the item count.
public class MaxLengthStringArrayQueryParam : TestBase
{
    public MaxLengthStringArrayQueryParam(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, ([FromQuery, MaxLength(2)] string[]? tag = null) => TypedResults.Ok(tag));
    }

    [Fact]
    public async Task returns_ok_when_within_the_limit()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?tag=a&tag=b");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task returns_bad_request_when_too_many_items()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?tag=a&tag=b&tag=c");

        // Assert
        await response.EnsureErrorFor("tag");
    }
}
