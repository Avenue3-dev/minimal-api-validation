namespace EndpointValidator.Tests.Combo;

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using EndpointValidator.Tests.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class QueryAndBody : TestBase
{
    public QueryAndBody(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Path, (
            [FromQuery] string query1,
            [FromQuery(Name = "q2"), Range(1, 5)] int query2,
            [FromBody] TestRecord body
        ) => TypedResults.Ok());
    }

    [Fact]
    public async Task returns_ok_when_query_params_and_body_are_valid()
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
            requestUri: $"{Path}?query1=some-value&q2=5"
        );
        request.Content = content;

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task returns_bad_request_when_query_param_is_missing()
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
            requestUri: $"{Path}?q2=5"
        );
        request.Content = content;

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("query1");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(9)]
    public async Task returns_bad_request_when_query_param_is_out_of_range(int value)
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
            requestUri: $"{Path}?query1=some-value&q2={value}"
        );
        request.Content = content;

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("q2");
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
            requestUri: $"{Path}?query1=some-value&q2=5"
        );
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
            requestUri: $"{Path}?q2=9"
        );
        request.Content = content;

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("name", "age", "query1", "q2");
    }
}
