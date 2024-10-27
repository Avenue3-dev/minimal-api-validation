namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.Combo;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class HeaderAndQuery : TestBase
{
    public HeaderAndQuery(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, (
            [FromHeader] string header1,
            [FromHeader(Name = "x-int"), Range(1, 5)] int header2,
            [FromQuery, MinLength(2)] string query1
        ) => TypedResults.Ok());
    }

    [Fact]
    public async Task returns_ok_when_headers_and_query_params_are_valid()
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: $"{Path}?query1=ok"
        );
        request.Headers.TryAddWithoutValidation("header1", "some-value");
        request.Headers.TryAddWithoutValidation("x-int", "5");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task returns_bad_request_when_header_is_missing()
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: $"{Path}?query1=ok"
        );
        request.Headers.TryAddWithoutValidation("x-int", "5");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("header1");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(9)]
    public async Task returns_bad_request_when_header_is_out_of_range(int value)
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: $"{Path}?query1=ok"
        );
        request.Headers.TryAddWithoutValidation("header1", "some-value");
        request.Headers.TryAddWithoutValidation("x-int", value.ToString());

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("x-int");
    }

    [Fact]
    public async Task returns_bad_request_when_query_param_is_missing()
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: $"{Path}"
        );
        request.Headers.TryAddWithoutValidation("header1", "some-value");
        request.Headers.TryAddWithoutValidation("x-int", "5");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("query1");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("a")]
    public async Task returns_bad_request_when_query_param_is_too_short(string value)
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: $"{Path}?query1={value}"
        );
        request.Headers.TryAddWithoutValidation("header1", "some-value");
        request.Headers.TryAddWithoutValidation("x-int", "5");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("query1");
    }

    [Fact]
    public async Task returns_bad_request_when_everything_is_invalid()
    {
        // Arrange// Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: $"{Path}?query1=o"
        );
        request.Headers.TryAddWithoutValidation("x-int", "9");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("query1", "header1", "x-int");
    }
}
