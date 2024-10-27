namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.CustomAttributes.GreaterThan;

using A3.MinimalApiValidation.ValidationAttributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class GreaterThanQueryDateTimeCompare : TestBase
{
    public GreaterThanQueryDateTimeCompare(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, (
            [FromQuery] DateTime query1,
            [FromQuery, GreaterThan(nameof(query1))] DateTime query2
        ) => TypedResults.Ok());
    }
    
    [Theory]
    [InlineData("2021-02-03")]
    [InlineData("2021-02-04")]
    [InlineData("2021-03-01")]
    public async Task returns_ok_when_query_params_are_valid(string value)
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?query1=2021-02-02&query2={value}");

        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    [Theory]
    [InlineData("2021-01-01")]
    [InlineData("2021-01-02")]
    [InlineData("2021-01-03")]
    [InlineData("2021-02-01")]
    [InlineData("2021-02-02")]
    public async Task returns_bad_request_when_query_params_is_lesser(string value)
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?query1=2021-02-02&query2={value}");

        // Assert
        await response.EnsureErrorFor("query2");
    }
}
