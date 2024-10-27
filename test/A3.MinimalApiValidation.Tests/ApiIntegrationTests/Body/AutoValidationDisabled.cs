namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.Body;

using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class AutoValidationDisabled : TestBase
{
    public AutoValidationDisabled(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void ConfigureOptions(EndpointValidatorOptions options)
    {
        options.PreferExplicitRequestBodyValidation = true;
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Path, ([FromBody] TestRecord body) => TypedResults.Ok());
    }

    [Theory]
    [InlineData("John", 0)]
    [InlineData("John", 101)]
    [InlineData(null, 30)]
    [InlineData("", 30)]
    [InlineData(" ", 30)]
    [InlineData("John", 30)]
    public async Task returns_ok_for_any_values_when_auto_validation_is_disabled(string? name, int age)
    {
        // Arrange
        var body = new
        {
            name = name,
            age = age,
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync(Path, content);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
