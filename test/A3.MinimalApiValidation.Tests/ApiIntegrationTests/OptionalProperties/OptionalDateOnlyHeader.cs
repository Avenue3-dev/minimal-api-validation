namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.OptionalProperties;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class OptionalDateOnlyHeader : TestBase
{
    public OptionalDateOnlyHeader(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, ([FromHeader(Name = "x-required")] DateOnly? header) => TypedResults.Ok());
    }

    [Theory]
    [InlineData("2022-02-22")]
    [InlineData("02/22/2022")]
    public async Task returns_ok_when_optional_header_is_valid(string header)
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("x-required", header);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task returns_ok_when_optional_header_is_missing()
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
    [InlineData("not-a-date")]
    [InlineData("123-456")]
    [InlineData("2022-02-22T22:22:22Z")]
    public async Task returns_bad_request_when_optional_header_is_not_a_date(string header)
    {
        // Arrange
        var request = new HttpRequestMessage(
            method: HttpMethod.Get,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("x-required", header);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        await response.EnsureErrorFor("x-required");
    }
}