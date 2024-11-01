namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.CustomAttributes.GreaterThanOrEqualTo;

using A3.MinimalApiValidation.ValidationAttributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class GreaterThanOrEqualToHeaderDateTimeCompare : TestBase
{
    public GreaterThanOrEqualToHeaderDateTimeCompare(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, (
            [FromHeader(Name = "header-1")] DateTime header1,
            [FromHeader(Name = "header-2"), GreaterThanOrEqualTo("header-1")] DateTime header2
        ) => TypedResults.Ok());
    }
    
    [Theory]
    [InlineData("2021-02-02")]
    [InlineData("2021-02-03")]
    [InlineData("2021-02-04")]
    [InlineData("2021-02-05")]
    public async Task returns_ok_when_header_is_valid(string value)
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("header-1", "2021-02-02");
        request.Headers.TryAddWithoutValidation("header-2", value);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    [Theory]
    [InlineData("2021-02-01")]
    [InlineData("2021-01-01")]
    [InlineData("2021-01-02")]
    [InlineData("2021-01-03")]
    public async Task returns_bad_request_when_header_is_lesser(string value)
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("header-1", "2021-02-02");
        request.Headers.TryAddWithoutValidation("header-2", value);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("header-2");
    }
}
