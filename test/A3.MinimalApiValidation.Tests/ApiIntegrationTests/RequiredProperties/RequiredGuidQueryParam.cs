namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.RequiredProperties;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class RequiredGuidQueryParam : TestBase
{
    public RequiredGuidQueryParam(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, ([FromQuery] Guid query) => TypedResults.Ok());
    }

    [Fact]
    public async Task returns_ok_when_required_query_param_is_valid()
    {
        // Arrange
        var guid = Guid.NewGuid();
        
        // Act
        var response = await Client.GetAsync($"{Path}?query={guid}");

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
    [InlineData("not-a-guid")]
    [InlineData("123")]
    [InlineData("123-456-789")]
    public async Task returns_bad_request_when_required_query_param_is_not_a_guid(string query)
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?query={query}");

        // Assert
        await response.EnsureErrorFor("query");
    }
}
