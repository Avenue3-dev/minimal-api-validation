namespace A3.MinimalApiValidation.Tests.BodyArray;

using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class FluentValidatorRegistered : TestBase
{
    public FluentValidatorRegistered(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Path, ([FromBody] TestRecord[] body) => TypedResults.Ok());
    }

    [Fact]
    public async Task returns_bad_request_for_one_invalid_item()
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
                age = 0,
            },
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync(Path, content);

        // Assert
        await response.EnsureErrorFor("item[1].age");
    }

    [Fact]
    public async Task returns_bad_request_for_multiple_invalid_items()
    {
        // Arrange
        var body = new[]
        {
            new
            {
                name = "",
                age = 30,
            },
            new
            {
                name = "Jane",
                age = 0,
            },
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync(Path, content);

        // Assert
        await response.EnsureErrorFor("item[0].name", "item[1].age");
    }

    [Fact]
    public async Task returns_bad_request_for_multiple_invalid_items_and_properties()
    {
        // Arrange
        var body = new[]
        {
            new
            {
                name = "",
                age = 0,
            },
            new
            {
                name = " ",
                age = 101,
            },
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync(Path, content);

        // Assert
        await response.EnsureErrorFor("item[0].name", "item[0].age", "item[1].name", "item[1].age");
    }

    [Fact]
    public async Task returns_ok_for_valid_items()
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
