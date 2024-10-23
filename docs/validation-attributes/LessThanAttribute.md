# LessThanAttribute

The `LessThanAttribute` validates that the value of the property is less than the specified value, or the value of another property.

It supports comparing `int`, `long`, `double`, `decimal`, `float`, and `DateTime` types.

Value check examples:

```csharp
[FromQuery, LessThan(5)] int query

//

[FromHeader, LessThan("2021-02-02")] DateTime header

//

[FromBody] TestModel body

public class TestModel
{
    public required int Item1 { get; init; }

    [LessThan(5)]
    public required int Item2 { get; init; }
}
```

Value comparison examples:

```csharp
app.MapGet("/test", (
    [FromQuery] int query1,
    [FromQuery, LessThan("query1")] int query2
) => { ... });

//

app.MapGet("/test", (
    [FromHeader(Name = "header-1")] DateTime header1,
    [FromHeader(Name = "header-2"), LessThan("header-1")] DateTime header2
) => { ... });

//

app.MapPost("/test", (
    [FromBody] TestModel body
) => { ... });

public class TestModel
{
    public required int Item1 { get; init; }

    [LessThan(nameof(Item1))]
    public required int Item2 { get; init; }
}
```
