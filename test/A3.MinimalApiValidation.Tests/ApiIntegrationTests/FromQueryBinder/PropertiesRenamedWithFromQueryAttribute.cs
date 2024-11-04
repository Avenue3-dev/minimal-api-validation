namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.FromQueryBinder;

using A3.MinimalApiValidation.Binders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class PropertiesRenamedWithFromQueryAttribute : TestBase
{
    public PropertiesRenamedWithFromQueryAttribute(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, (FromQuery<TestRecordRenamed> query) => TypedResults.Ok(query));
    }
    
    [Fact]
    public async Task returns_bad_request_for_invalid_state()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?has-cake=true&eaten=true");

        // Assert
        await response.EnsureErrorFor("");
    }
    
    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public async Task returns_ok_for_valid_values(bool cake, bool eaten)
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{Path}?has-cake={cake}&eaten={eaten}");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
