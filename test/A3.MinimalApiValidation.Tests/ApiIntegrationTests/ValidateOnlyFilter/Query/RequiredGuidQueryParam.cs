namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.ValidateOnlyFilter.Query;

using System.Net;
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
        app.MapGet(Path, ([FromQuery] Guid query) => Results.StatusCode(500)).WithValidateOnly();
    }

    [Fact]
    public async Task returns_accepted_when_required_query_param_is_valid()
    {
        // Arrange
        var guid = Guid.NewGuid();
        
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: $"{Path}?query={guid}"
        );
        request.Headers.TryAddWithoutValidation("x-validate-only", "true");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
    }

    [Fact]
    public async Task returns_bad_request_when_required_query_param_is_missing()
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: $"{Path}"
        );
        request.Headers.TryAddWithoutValidation("x-validate-only", "true");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("query");
    }

    [Fact]
    public async Task returns_bad_request_when_required_query_param_is_empty()
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: $"{Path}?query="
        );
        request.Headers.TryAddWithoutValidation("x-validate-only", "true");

        // Act
        var response = await Client.SendAsync(request);

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
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: $"{Path}?query={query}"
        );
        request.Headers.TryAddWithoutValidation("x-validate-only", "true");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("query");
    }
}
