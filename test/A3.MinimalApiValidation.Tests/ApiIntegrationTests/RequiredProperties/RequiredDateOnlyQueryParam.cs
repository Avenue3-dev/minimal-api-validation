namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.RequiredProperties;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class RequiredDateOnlyQueryParam : TestBase
{
    public RequiredDateOnlyQueryParam(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, ([FromQuery] DateOnly query) => TypedResults.Ok());
    }

    [Theory]
    [InlineData("2022-02-22")]
    [InlineData("02/22/2022")]
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
    public async Task returns_bad_request_when_required_query_param_is_empty()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?query=");

        // Assert
        await response.EnsureErrorFor("query");
    }

    [Theory]
    [InlineData("not-a-date")]
    [InlineData("123-456")]
    [InlineData("2022-02-22T22:22:22Z")]
    public async Task returns_bad_request_when_required_query_param_is_not_a_date(string query)
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?query={query}");

        // Assert
        await response.EnsureErrorFor("query");
    }
}