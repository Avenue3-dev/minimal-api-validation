namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.Collections;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

// A non-nullable collection parameter is required: an absent value is an error.
// (Note: the minimal api framework only binds arrays / StringValues for [FromQuery]; use the
// FromQuery<T> model binder for List<T>/IEnumerable<T> collection properties.)
public class RequiredStringArrayQueryParam : TestBase
{
    public RequiredStringArrayQueryParam(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, ([FromQuery] string[] tag) => TypedResults.Ok(tag));
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

    [Fact]
    public async Task returns_bad_request_when_absent()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}");

        // Assert
        await response.EnsureErrorFor("tag");
    }
}
