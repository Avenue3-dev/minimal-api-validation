namespace EndpointValidator.Tests.BodyArray;

using System.Text;
using System.Text.Json;
using EndpointValidator.Tests.Client;
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
        app.MapPost(Path, ([FromBody] TestRecord[] body) => TypedResults.Ok());
    }

    [Fact]
    public async Task returns_ok_for_any_values_when_auto_validation_is_disabled()
    {
        // Arrange
        var body = new[]
        {
            new
            {
                name = "John",
                age = 30,
            },
            new
            {
                name = "Jane",
                age = 55,
            },
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync(Path, content);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
