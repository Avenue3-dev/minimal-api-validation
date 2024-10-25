namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.FromQueryBinder;

using A3.MinimalApiValidation.Binders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
        app.MapGet(Path, (FromQuery<TestRecordAnnotated> query) => TypedResults.Ok(query));
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task returns_bad_request_for_invalid_name(string? name)
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?name={name}&age=55");

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
        // Act
        var response = await Client.GetAsync($"{Path}?name=Harry&age={age}");

        // Assert
        await response.EnsureErrorFor("age");
    }
    
    [Fact]
    public async Task returns_bad_request_for_invalid_name_and_age()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}");

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
        // Act
        var response = await Client.GetAsync($"{Path}?name={name}&age={age}");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
