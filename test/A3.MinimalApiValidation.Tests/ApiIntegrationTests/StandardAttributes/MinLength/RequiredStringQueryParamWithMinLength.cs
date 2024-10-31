namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.StandardAttributes.MinLength;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class RequiredStringQueryParamWithMinLength : TestBase
{
    public RequiredStringQueryParamWithMinLength(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, ([FromQuery, MinLength(2)] string query) => TypedResults.Ok());
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("abc")]
    [InlineData("this is fine")]
    [InlineData("this is even longer but still passes the validation rules")]
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
    
    [Fact]
    public async Task returns_bad_request_when_required_query_param_is_too_short()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?query=a");

        // Assert
        await response.EnsureErrorFor("query");
    }
}
