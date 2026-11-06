namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.Collections;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class StringArrayHeader : TestBase
{
    public StringArrayHeader(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, ([FromHeader(Name = "x-tag")] string[]? tag = null) => TypedResults.Ok(tag));
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
    public async Task returns_ok_for_multiple_header_values()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, Path);
        request.Headers.Add("x-tag", "a");
        request.Headers.Add("x-tag", "b");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
