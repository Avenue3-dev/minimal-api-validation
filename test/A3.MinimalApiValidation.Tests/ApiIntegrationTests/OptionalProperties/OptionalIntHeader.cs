namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.OptionalProperties;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class OptionalIntHeader : TestBase
{
    public OptionalIntHeader(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, ([FromHeader(Name = "x-optional")] int? header) => TypedResults.Ok());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(123)]
    public async Task returns_ok_when_required_header_is_valid(int header)
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("x-optional", header.ToString());

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task returns_ok_when_required_header_is_missing()
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData("not-an-int")]
    [InlineData("123-456-789")]
    public async Task returns_bad_request_when_required_header_is_not_an_int(string header)
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("x-optional", header);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("x-optional");
    }
}
