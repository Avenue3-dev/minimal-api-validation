namespace A3.MinimalApiValidation.Tests.CustomAttributes.LessThan;

using System.Text;
using System.Text.Json;
using A3.MinimalApiValidation.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class LessThanBodyIntFromString : TestBase
{
    public LessThanBodyIntFromString(WebApplicationFactory<Program> factory) : base(factory)
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
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task returns_ok_when_model_is_valid(int value)
    {
        // Arrange
        var body = new
        {
            item1 = 5,
            item2 = value,
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        // Act
        var response = await Client.PostAsync(Path, content);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    public async Task returns_bad_request_when_model_property_is_greater(int value)
    {
        // Arrange
        var body = new
        {
            item1 = 5,
            item2 = value,
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        // Act
        var response = await Client.PostAsync(Path, content);

        // Assert
        await response.EnsureErrorFor("item2");
    }

    public class TestModel
    {
        public required int Item1 { get; init; }
        
        [LessThan("3")]
        public required int Item2 { get; init; }
    }
}
