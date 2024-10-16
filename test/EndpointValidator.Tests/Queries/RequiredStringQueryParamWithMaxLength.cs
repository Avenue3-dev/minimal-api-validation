namespace EndpointValidator.Tests.Queries;

using System.ComponentModel.DataAnnotations;
using EndpointValidator.Tests.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class RequiredStringQueryParamWithMaxLength : TestBase
{
    public RequiredStringQueryParamWithMaxLength(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, ([FromQuery, MaxLength(5)] string query) => TypedResults.Ok());
    }

    [Theory]
    [InlineData("a")]
    [InlineData("ab")]
    [InlineData("abc")]
    [InlineData("a123")]
    [InlineData("a1234")]
    public async Task returns_ok_when_required_query_param_is_valid(string query)
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?query={query}");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task returns_bad_request_when_required_query_param_is_missing()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}");

        // Assert
        await response.EnsureErrorFor("query");
    }

    [Fact]
    public async Task returns_ok_when_required_query_param_is_empty()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?query=");

        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    [Theory]
    [InlineData("a12345")]
    [InlineData("too long")]
    [InlineData("this is also way too long")]
    public async Task returns_bad_request_when_required_query_param_is_too_long(string value)
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?query={value}");

        // Assert
        await response.EnsureErrorFor("query");
    }
}
