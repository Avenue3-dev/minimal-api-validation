namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.CustomAttributes.Min;

using A3.MinimalApiValidation.ValidationAttributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class MinHeaderIntFromString: TestBase
{
    public MinHeaderIntFromString(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, (
            [FromHeader(Name = "x-header-1"), Min("3")] int header1
        ) => TypedResults.Ok());
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public async Task returns_ok_when_header_is_valid(int value)
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("x-header-1", value.ToString());
        
        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData(2)]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task returns_bad_request_when_header_is_invalid(int value)
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("x-header-1", value.ToString());
        
        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("x-header-1");
    }
}
