namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.Combo;

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class HeaderAndBody : TestBase
{
    public HeaderAndBody(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Path, (
            [FromHeader] string header1,
            [FromHeader(Name = "x-int"), Range(1, 5)] int header2,
            [FromBody] TestRecord body
        ) => TypedResults.Ok());
    }

    [Fact]
    public async Task returns_ok_when_headers_and_body_are_valid()
    {
        // Arrange
        var body = new
        {
            name = "John",
            age = 30,
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(
            method: HttpMethod.Post,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("header1", "some-value");
        request.Headers.TryAddWithoutValidation("x-int", "5");
        request.Content = content;

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task returns_bad_request_when_header_is_missing()
    {
        // Arrange
        var body = new
        {
            name = "John",
            age = 30,
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(
            method: HttpMethod.Post,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("x-int", "5");
        request.Content = content;

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
        var body = new
        {
            name = "John",
            age = 30,
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(
            method: HttpMethod.Post,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("header1", "some-value");
        request.Headers.TryAddWithoutValidation("x-int", value.ToString());
        request.Content = content;

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("x-int");
    }

    [Fact]
    public async Task returns_bad_request_when_body_is_invalid()
    {
        // Arrange
        var body = new
        {
            name = "",
            age = 30,
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(
            method: HttpMethod.Post,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("header1", "some-value");
        request.Headers.TryAddWithoutValidation("x-int", "5");
        request.Content = content;

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("name");
    }

    [Fact]
    public async Task returns_bad_request_when_everything_is_invalid()
    {
        // Arrange
        var body = new
        {
            name = "",
            age = 0,
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(
            method: HttpMethod.Post,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("x-int", "9");
        request.Content = content;

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("name", "age", "header1", "x-int");
    }
}
