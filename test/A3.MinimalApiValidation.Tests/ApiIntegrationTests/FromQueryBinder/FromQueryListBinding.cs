namespace A3.MinimalApiValidation.Tests.ApiIntegrationTests.FromQueryBinder;

using System.Net.Http.Json;
using A3.MinimalApiValidation.Binders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

public class FromQueryListBinding : TestBase
{
    public FromQueryListBinding(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    protected override void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Path, (FromQuery<QueryModel> query) => TypedResults.Ok(query.Value));
    }

    public record QueryModel(
        string Name,
        List<Guid> Ids,
        string[] Items,
        IEnumerable<int> Numbers,
        IReadOnlyCollection<DateTime> Dates);
    
    [Fact]
    public async Task properties_are_bound_correctly()
    {
        // Arrange
        var name = "John";
        var id1 = "B6DFFA8C-AE7D-4C39-AFEF-7B11FECA6C65";
        var id2 = "CDB56FE6-727B-4D86-A768-0277AB750527";
        var item1 = "Hello";
        var item2 = "World";
        var number1 = 42;
        var number2 = 73;
        var date1 = "2022-01-01T00:00:00Z";
        var date2 = "2022-01-02T00:00:00Z";
        
        // Act
        var response = await Client.GetAsync($"{Path}" +
            $"?name={name}" +
            $"&ids={id1}&ids={id2}" +
            $"&items={item1}&items={item2}" +
            $"&numbers={number1}&numbers={number2}" +
            $"&dates={date1}&dates={date2}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<QueryModel>();
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(2, result.Ids.Count);
        Assert.Equal(Guid.Parse(id1), result.Ids[0]);
        Assert.Equal(Guid.Parse(id2), result.Ids[1]);
        Assert.Equal(2, result.Items.Length);
        Assert.Equal(item1, result.Items[0]);
        Assert.Equal(item2, result.Items[1]);
        Assert.Equal(2, result.Numbers.Count());
        Assert.Equal(number1, result.Numbers.First());
        Assert.Equal(number2, result.Numbers.Last());
        Assert.Equal(2, result.Dates.Count);
        Assert.Equal(DateTime.Parse(date1), result.Dates.First());
        Assert.Equal(DateTime.Parse(date2), result.Dates.Last());
    }
}
