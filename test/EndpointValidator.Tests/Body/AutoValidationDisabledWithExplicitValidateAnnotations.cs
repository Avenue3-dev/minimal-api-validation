namespace EndpointValidator.Tests.Body;

using System.Text;
using System.Text.Json;
using EndpointValidator.Tests.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class AutoValidationDisabledWithExplicitValidateAnnotations : TestBase
{
    public AutoValidationDisabledWithExplicitValidateAnnotations(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override bool RegisterValidator => false;

    protected override void ConfigureOptions(EndpointValidatorOptions options)
    {
        options.PreferExplicitRequestBodyValidation = true;
        options.FallbackToDataAnnotations = true;
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Path, ([FromBody] TestRecordAnnotated body) => TypedResults.Ok()).Validate<TestRecordAnnotated>();
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task returns_bad_request_for_invalid_name(string? name)
    {
        // Arrange
        var body = new
        {
            name = name,
            age = 30,
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync(Path, content);

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
        var body = new
        {
            name = "John",
            age = age,
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync(Path, content);

        // Assert
        await response.EnsureErrorFor("age");
    }
    
    [Fact]
    public async Task returns_bad_request_for_invalid_name_and_age()
    {
        // Arrange
        var body = new
        {
            name = "",
            age = 0,
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync(Path, content);

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
    public async Task returns_ok_for_valid_values(string name, int age)
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
