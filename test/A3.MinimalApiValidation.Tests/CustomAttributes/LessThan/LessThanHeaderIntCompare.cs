namespace A3.MinimalApiValidation.Tests.CustomAttributes.LessThan;

using A3.MinimalApiValidation.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class LessThanHeaderIntCompare : TestBase
{
    public LessThanHeaderIntCompare(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, (
            [FromHeader(Name = "header-1")] int header1,
            [FromHeader(Name = "header-2"), LessThan("header-1")] int header2
        ) => TypedResults.Ok());
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public async Task returns_ok_when_header_is_valid(int value)
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("header-1", "5");
        request.Headers.TryAddWithoutValidation("header-2", value.ToString());

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    [Theory]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    public async Task returns_bad_request_when_header_is_greater(int value)
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("header-1", "5");
        request.Headers.TryAddWithoutValidation("header-2", value.ToString());

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("header-2");
    }
}
