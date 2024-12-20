namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.CustomAttributes.Min;

using A3.MinimalApiValidation.ValidationAttributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class MinQueryInt: TestBase
{
    public MinQueryInt(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, (
            [FromQuery, Min(3)] int query1
        ) => TypedResults.Ok());
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public async Task returns_ok_when_query_param_is_valid(int value)
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?query1={value}");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData(2)]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task returns_bad_request_when_query_param_is_invalid(int value)
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?query1={value}");

        // Assert
        await response.EnsureErrorFor("query1");
    }
}
