namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.CustomAttributes.LessThanOrEqualTo;

using System.Text;
using System.Text.Json;
using A3.MinimalApiValidation.ValidationAttributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class LessThanOrEqualToBodyDateTimeCompare : TestBase
{
    public LessThanOrEqualToBodyDateTimeCompare(WebApplicationFactory<Program> factory) : base(factory)
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
    [InlineData("2021-02-02")]
    [InlineData("2021-02-01")]
    [InlineData("2021-01-02")]
    [InlineData("2020-02-02")]
    public async Task returns_ok_when_model_is_valid(string value)
    {
        // Arrange
        var body = new
        {
            item1 = "2021-02-02",
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
    [InlineData("2021-02-03")]
    [InlineData("2021-02-04")]
    [InlineData("2021-02-05")]
    public async Task returns_bad_request_when_model_property_is_greater(string value)
    {
        // Arrange
        var body = new
        {
            item1 = "2021-02-02",
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
        public required DateTime Item1 { get; init; }
        
        [LessThanOrEqualTo(nameof(Item1))]
        public required DateTime Item2 { get; init; }
    }
}
