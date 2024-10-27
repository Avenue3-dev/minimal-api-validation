namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.CustomAttributes.Min;

using A3.MinimalApiValidation.ValidationAttributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class MinQueryDateTime: TestBase
{
    public MinQueryDateTime(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, (
            [FromQuery, Min("2022-02-22")] DateTime query1
        ) => TypedResults.Ok());
    }

    [Theory]
    [InlineData("2022-02-22")]
    [InlineData("2022-02-23")]
    [InlineData("2022-02-24")]
    public async Task returns_ok_when_query_param_is_valid(string value)
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?query1={value}");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData("2022-02-21")]
    [InlineData("2022-02-20")]
    [InlineData("2022-02-19")]
    public async Task returns_bad_request_when_query_param_is_invalid(string value)
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?query1={value}");

        // Assert
        await response.EnsureErrorFor("query1");
    }
}
