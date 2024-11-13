namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.ValidateOnlyFilter.Header;

using System.Net;
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
        app.MapGet(Path, ([FromHeader(Name = "x-required")] Guid header) => Results.StatusCode(500)).WithValidateOnly();
    }

    [Fact]
    public async Task returns_accepted_when_required_header_is_valid()
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("x-required", Guid.NewGuid().ToString());
        request.Headers.TryAddWithoutValidation("x-validate-only", "true");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
    }

    [Fact]
    public async Task returns_bad_request_when_required_header_is_missing()
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("x-validate-only", "true");

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
        request.Headers.TryAddWithoutValidation("x-validate-only", "true");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("x-required");
    }
}
