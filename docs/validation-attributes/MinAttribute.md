# MinAttribute

The `MinAttribute` validates that the value of the property is greater than or equal to the specified value.

It supports comparing `int`, `long`, `double`, `decimal`, and `float`, and `DateTime` types.

## Example usage:

```csharp
// ?query=5
[FromQuery, Min(5)] int query

// header: 2021-02-02
[FromHeader, Min("2021-02-02")] DateTime header

// body: { "Item1": 5 }
[FromBody] TestModel body

public class TestModel
{
    [Min("5")]
    public required int Item1 { get; init; }
}
```
