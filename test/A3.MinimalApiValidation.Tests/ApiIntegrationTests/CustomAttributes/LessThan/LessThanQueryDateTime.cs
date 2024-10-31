namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.CustomAttributes.LessThan;

using A3.MinimalApiValidation.ValidationAttributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class LessThanQueryDateTime : TestBase
{
    public LessThanQueryDateTime(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, (
            [FromQuery] int query1,
            [FromQuery, LessThan("2021-02-02")] DateTime query2
        ) => TypedResults.Ok());
    }

    [Theory]
    [InlineData("2021-01-01")]
    [InlineData("2021-01-02")]
    [InlineData("2021-01-03")]
    public async Task returns_ok_when_query_params_are_valid(string value)
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?query1=5&query2={value}");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData("2021-02-02")]
    [InlineData("2021-02-03")]
    [InlineData("2021-02-04")]
    public async Task returns_bad_request_when_query_params_is_greater(string value)
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?query1=5&query2={value}");

        // Assert
        await response.EnsureErrorFor("query2");
    }
}
