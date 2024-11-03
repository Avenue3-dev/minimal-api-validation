namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.RequiredProperties;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class RequiredGuidHeader : TestBase
{
    public RequiredGuidHeader(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, ([FromHeader(Name = "x-required")] Guid header) => TypedResults.Ok());
    }

    [Fact]
    public async Task returns_ok_when_required_header_is_valid()
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("x-required", Guid.NewGuid().ToString());

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task returns_bad_request_when_required_header_is_missing()
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("x-required");
    }
    
    [Theory]
    [InlineData("not-a-guid")]
    [InlineData("123")]
    [InlineData("123-456-789")]
    public async Task returns_bad_request_when_required_header_is_not_a_guid(string header)
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("x-required", header);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("x-required");
    }
}
