namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.CustomAttributes.LessThan;

using A3.MinimalApiValidation.ValidationAttributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class LessThanQueryIntCompare : TestBase
{
    public LessThanQueryIntCompare(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, (
            [FromQuery] int query1,
            [FromQuery, LessThan(nameof(query1))] int query2
        ) => TypedResults.Ok());
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public async Task returns_ok_when_query_params_are_valid(int value)
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?query1=5&query2={value}");

        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    [Theory]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    public async Task returns_bad_request_when_query_params_is_greater(int value)
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?query1=5&query2={value}");

        // Assert
        await response.EnsureErrorFor("query2");
    }
}
