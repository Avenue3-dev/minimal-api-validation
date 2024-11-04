# MaxAttribute

The `MaxAttribute` validates that the value of the property is less than or equal to the specified value.

It supports comparing `int`, `long`, `double`, `decimal`, and `float`, and `DateTime` types.

## Example usage:

```csharp
// ?query=5
[FromQuery, Max(5)] int query

// header: 2021-02-02
[FromHeader, Max("2021-02-02")] DateTime header

// body: { "Item1": 5 }
[FromBody] TestModel body

public class TestModel
{
    [Max("5")]
    public required int Item1 { get; init; }
}
```

> **Note:** The `MaxAttribute` performs the same validation as the `LessThanOrEqualAttribute` but excludes property comparison. Therefore this may be preferred when performance is a concern.
