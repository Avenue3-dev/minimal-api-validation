namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.CustomAttributes.Min;

using System.Text;
using System.Text.Json;
using A3.MinimalApiValidation.ValidationAttributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class MinBodyDateTime: TestBase
{
    public MinBodyDateTime(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    
    protected override bool RegisterValidator => false;

    protected override void ConfigureOptions(EndpointValidatorOptions options)
    {
        options.FallbackToDataAnnotations = true;
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Path, (
            [FromBody] TestModel body
        ) => TypedResults.Ok());
    }

    [Theory]
    [InlineData("2022-02-22")]
    [InlineData("2022-02-23")]
    [InlineData("2022-02-24")]
    public async Task returns_ok_when_model_is_valid(string value)
    {
        // Arrange
        var body = new
        {
            item1 = value,
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        // Act
        var response = await Client.PostAsync(Path, content);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData("2022-02-21")]
    [InlineData("2022-02-20")]
    [InlineData("2022-02-19")]
    public async Task returns_bad_request_when_model_property_is_invalid(string value)
    {
        // Arrange
        var body = new
        {
            item1 = value,
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        // Act
        var response = await Client.PostAsync(Path, content);

        // Assert
        await response.EnsureErrorFor("item1");
    }
    
    public class TestModel
    {
        [Min("2022-02-22")]
        public required DateTime Item1 { get; init; }
    }
}
