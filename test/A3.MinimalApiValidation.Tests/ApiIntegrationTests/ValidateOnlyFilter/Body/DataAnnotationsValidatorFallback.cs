namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.ValidateOnlyFilter.Body;

using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class DataAnnotationsValidatorFallback : TestBase
{
    public DataAnnotationsValidatorFallback(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    
    protected override bool RegisterValidator => false;

    protected override void ConfigureOptions(EndpointValidatorOptions options)
    {
        options.FallbackToDataAnnotations = true;
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Path, ([FromBody] TestRecordAnnotated body) => Results.StatusCode(500)).WithValidateOnly();
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task returns_bad_request_for_invalid_name(string? name)
    {
        // Arrange
        var body = JsonSerializer.Serialize(new
        {
            name = name,
            age = 30,
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
        await response.EnsureErrorFor("name");
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(101)]
    [InlineData(1001)]
    public async Task returns_bad_request_for_invalid_age(int age)
    {
        // Arrange
        var body = JsonSerializer.Serialize(new
        {
            name = "John",
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
        await response.EnsureErrorFor("age");
    }
    
    [Fact]
    public async Task returns_bad_request_for_invalid_name_and_age()
    {
        // Arrange
        var body = JsonSerializer.Serialize(new
        {
            name = "",
            age = 0,
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
        await response.EnsureErrorFor("name", "age");
    }
    
    [Theory]
    [InlineData("John", 1)]
    [InlineData("John", 30)]
    [InlineData("John", 99)]
    [InlineData("Jane", 1)]
    [InlineData("Jane", 30)]
    [InlineData("Jane", 99)]
    public async Task returns_accepted_for_valid_values(string name, int age)
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
