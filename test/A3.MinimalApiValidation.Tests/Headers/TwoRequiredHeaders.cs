namespace A3.MinimalApiValidation.Tests.Headers;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class TwoRequiredHeaders : TestBase
{
    public TwoRequiredHeaders(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, (
            [FromHeader(Name = "x-required-1")] string header1,
            [FromHeader] int header2
            ) => TypedResults.Ok());
    }

    [Theory]
    [InlineData("some value", 1)]
    [InlineData("some other value", 2)]
    [InlineData("something else 123", 123)]
    public async Task returns_ok_when_required_header_is_valid(string header1, int header2)
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("x-required-1", header1);
        request.Headers.TryAddWithoutValidation("header2", header2.ToString());

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task returns_bad_request_when_first_required_header_is_missing()
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("header2", "123");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("x-required-1");
    }

    [Fact]
    public async Task returns_bad_request_when_second_required_header_is_missing()
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("x-required-1", "header-1");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("header2");
    }

    [Fact]
    public async Task returns_bad_request_when_both_required_headers_are_missing()
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("x-required-1", "header2");
    }
}
