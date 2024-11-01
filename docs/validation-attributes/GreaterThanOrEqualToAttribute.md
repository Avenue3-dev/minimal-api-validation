# GreaterThanOrEqualToAttribute

The `GreaterThanOrEqualToAttribute` validates that the value of the property is greater than or equal to the specified value, or the value of another property.

It supports comparing `int`, `long`, `double`, `decimal`, `float`, and `DateTime` types.

## Greater Than or Equal to Value
Provide a number or valid ISO 8601 date string to compare the value with:

```csharp
// ?query=6
[FromQuery, GreaterThanOrEqualTo(5)] int query

// header: 2021-02-03
[FromHeader, GreaterThanOrEqualTo("2021-02-02")] DateTime header

// body: { "Item1": 6 }
[FromBody] TestModel body

public class TestModel
{
    [GreaterThanOrEqualTo(5)]
    public required int Item1 { get; init; }
}
```

## Greater Than or Equal to Property
Provide the name of the property to compare the value with. The property must be of the same type.

If the other property is a header or query parameter, the name provided must match the header or query parameter on the incoming request, not the c# property name, for example:
- `[FromQuery] int query1`, other property name: "query1"
- `[FromQuery(Name = "query-1")] int query1`, other property name: "query-1"
- `[FromHeader] DateTime header1`, other property name: "header1"
- `[FromHeader(Name = "header-1")] DateTime header1`, other property name: "header-1"

For body properties, the name provided must match the c# property name.

```csharp
// ?query1=5&query2=6
app.MapGet("/test", (
    [FromQuery] int query1,
    [FromQuery, GreaterThanOrEqualTo("query1")] int query2
) => { ... });

// header-1: 2021-02-01, header-2: 2021-02-02
app.MapGet("/test", (
    [FromHeader(Name = "header-1")] DateTime header1,
    [FromHeader(Name = "header-2"), GreaterThanOrEqualTo("header-1")] DateTime header2
) => { ... });

// body: { "Item1": 5, "Item2": 6 }
app.MapPost("/test", (
    [FromBody] TestModel body
) => { ... });

public class TestModel
{
    public required int Item1 { get; init; }

    [GreaterThanOrEqualTo(nameof(Item1))]
    public required int Item2 { get; init; }
}
```
