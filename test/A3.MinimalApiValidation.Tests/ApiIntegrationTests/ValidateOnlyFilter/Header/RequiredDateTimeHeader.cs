namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.ValidateOnlyFilter.Header;

using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class RequiredDateTimeHeader : TestBase
{
    public RequiredDateTimeHeader(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, ([FromHeader(Name = "x-required")] DateTime header) => Results.StatusCode(500)).WithValidateOnly();
    }

    [Theory]
    [InlineData("2022-02-22")]
    [InlineData("2022-02-22T22:22:22")]
    [InlineData("2022-02-22T22:22:22Z")]
    [InlineData("2022-02-22T22:22:22.123")]
    public async Task returns_accepted_when_required_header_is_valid(string header)
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
    [InlineData("not-a-date")]
    [InlineData("123-456")]
    public async Task returns_bad_request_when_required_header_is_not_a_date(string header)
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
