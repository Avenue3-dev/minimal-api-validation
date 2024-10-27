namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.Combo;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class TwoRequiredQueryParams : TestBase
{
    public TwoRequiredQueryParams(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, (
            [FromQuery] string query1,
            [FromQuery(Name = "q2")] int query2
        ) => TypedResults.Ok());
    }

    [Theory]
    [InlineData("value", 1)]
    [InlineData("some-other-value", 2)]
    [InlineData("something-else-123", 123)]
    public async Task returns_ok_when_required_query_param_is_valid(string query1, int query2)
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?query1={query1}&q2={query2}");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task returns_bad_request_when_first_required_query_param_is_missing()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?q2=123");

        // Assert
        await response.EnsureErrorFor("query1");
    }

    [Fact]
    public async Task returns_bad_request_when_second_required_query_param_is_missing()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?query1=value-1");

        // Assert
        await response.EnsureErrorFor("q2");
    }

    [Fact]
    public async Task returns_bad_request_when_both_required_query_params_are_missing()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}");

        // Assert
        await response.EnsureErrorFor("query1", "q2");
    }
}
