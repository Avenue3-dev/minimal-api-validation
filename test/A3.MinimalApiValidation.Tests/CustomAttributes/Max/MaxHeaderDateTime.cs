namespace A3.MinimalApiValidation.Tests.CustomAttributes.Max;

using A3.MinimalApiValidation.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class MaxHeaderDateTime: TestBase
{
    public MaxHeaderDateTime(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, (
            [FromHeader(Name = "x-header-1"), Max("2022-02-22")] DateTime header1
        ) => TypedResults.Ok());
    }

    [Theory]
    [InlineData("2022-02-22")]
    [InlineData("2022-02-21")]
    [InlineData("2022-02-20")]
    [InlineData("2022-02-19")]
    public async Task returns_ok_when_header_is_valid(string value)
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("x-header-1", value);
        
        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData("2022-02-23")]
    [InlineData("2022-02-24")]
    [InlineData("2022-02-25")]
    public async Task returns_bad_request_when_header_is_invalid(string value)
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("x-header-1", value);
        
        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("x-header-1");
    }
}
