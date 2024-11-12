namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.ValidateOnlyFilter.Body;

using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class NoValidatorRegistered : TestBase
{
    public NoValidatorRegistered(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    
    protected override bool RegisterValidator => false;

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Path, ([FromBody] TestRecord body) => Results.StatusCode(500)).WithValidateOnly();
    }

    [Theory]
    [InlineData("John", 0)]
    [InlineData("John", 101)]
    [InlineData(null, 30)]
    [InlineData("", 30)]
    [InlineData(" ", 30)]
    [InlineData("John", 30)]
    public async Task returns_accepted_for_any_values_when_no_validator_is_registered(string? name, int age)
    {
        // Arrange
        var body = JsonSerializer.Serialize(new
        {
            name = name,
            age = age,
        });
        
        var request = new HttpRequestMessage(
            method: HttpMethod.Post,
            requestUri: Path
        );
        request.Headers.TryAddWithoutValidation("x-validate-only", "true");
        request.Content = new StringContent(body, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
    }
}
