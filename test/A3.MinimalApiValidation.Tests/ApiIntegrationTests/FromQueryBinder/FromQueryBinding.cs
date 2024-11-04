namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.FromQueryBinder;

using System.Net.Http.Json;
using A3.MinimalApiValidation.Binders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class FromQueryBinding : TestBase
{
    public FromQueryBinding(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, (FromQuery<QueryModel> query) => TypedResults.Ok(query.Value));
    }

    public record QueryModel(
        string Name,
        int Age,
        [property: FromQuery(Name = "has-cake")] bool HasCake,
        DateTime Time,
        [property: FromQuery(Name = "user-id")]Guid Id,
        long? OptionalLong,
        string? OptionalString
    );
    
    [Theory]
    [InlineData(null, null)]
    [InlineData(42L, "Hello")]
    [InlineData(0L, "World")]
    [InlineData(1L, "42")]
    public async Task properties_are_bound_correctly(long? optionalLong, string? optionalString)
    {
        // Arrange
        var name = "John";
        var age = 42;
        var hasCake = true;
        var time = "2022-01-01T00:00:00Z";
        var id = "B6DFFA8C-AE7D-4C39-AFEF-7B11FECA6C65";
        
        // Act
        var response = await Client.GetAsync($"{Path}" +
            $"?name={name}" +
            $"&age={age}" +
            $"&has-cake={hasCake}" +
            $"&time={time}" +
            $"&user-id={id}" +
            $"&optionalLong={optionalLong}" +
            $"&optionalString={optionalString}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<QueryModel>();
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(age, result.Age);
        Assert.True(result.HasCake);
        Assert.Equal(DateTime.Parse(time), result.Time);
        Assert.Equal(Guid.Parse(id), result.Id);
        Assert.Equal(optionalLong, result.OptionalLong);
        Assert.Equal(optionalString ?? string.Empty, result.OptionalString);
    }
}
