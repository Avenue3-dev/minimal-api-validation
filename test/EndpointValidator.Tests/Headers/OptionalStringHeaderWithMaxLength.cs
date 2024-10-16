namespace EndpointValidator.Tests.Headers;

using System.ComponentModel.DataAnnotations;
using EndpointValidator.Tests.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class OptionalStringHeaderWithMaxLength : TestBase
{
    public OptionalStringHeaderWithMaxLength(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, ([FromHeader(Name = "x-optional"), MaxLength(5)] string? header) => TypedResults.Ok());
    }

    [Theory]
    [InlineData("a")]
    [InlineData("ab")]
    [InlineData("abc")]
    [InlineData("a123")]
    [InlineData("a1234")]
    public async Task returns_ok_when_optional_header_is_valid(string header)
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
    [InlineData("a12345")]
    [InlineData("too long")]
    [InlineData("this is also way too long")]
    public async Task returns_bad_request_when_optional_header_is_out_of_range(string header)
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
