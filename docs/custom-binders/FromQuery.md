# FromQuery\<T\>
Use `FromQuery<T>` instead of `[FromQuery]` to automatically bind and validate a group of dependent query parameters.

## Map multiple query parameters to a single object

You can map multiple query parameters to a single object using the `FromQuery<T>` binder. Simply register a route handler that accepts a `FromQuery<T>` parameter, where `T` is a type that defines the query parameters you want to bind.

> The `FromQuery<T>` model binding does not support nested objects.

```csharp

To validate, just register a FluentValidation validator for the same type.

```csharp
app.MapGet("/test-query-model", (FromQuery<TestRecord> test) => test);

public record TestRecord(string Name, bool HasCake, bool HasEatenIt);

public class TestRecordValidator : AbstractValidator<TestRecord>
{
    public TestRecordValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x)
            .Must(x => x is not {HasCake: true, HasEatenIt: true})
            .WithMessage("You can't have your cake and eat it too!");
    }
}
```
Example: `GET /test-query-model?name=John&hasCake=true&hasEatenIt=false`

### Partial Matching

If required, you can match just a subset of query parameters:

```csharp
// query parameters are matched to the given object and validated automatically based on any registered FluentValidation validator
app.MapGet("/test-query-model", (FromQuery<TestRecord> test, [FromQuery] string name) => test);

public record TestRecord(bool HasCake, bool HasEatenIt);

public class TestRecordValidator : AbstractValidator<TestRecord>
{
    public TestRecordValidator()
    {
        RuleFor(x => x)
            .Must(x => x is not {HasCake: true, HasEatenIt: true})
            .WithMessage("You can't have your cake and eat it too!");
    }
}
```
Example: `GET /test-query-model?name=John&hasCake=true&hasEatenIt=false`

### Collections

Collections are supported when providing multiple values for the same query parameter:

```csharp
app.MapGet("/test-query-model", (FromQuery<TestRecord> test, [FromQuery] string name) => test);

public record TestRecord(List<string> Items, int[] Ids);
```
Example: `GET /test-query-model?items=apple&items=banana&ids=1&ids=2&ids=3`

### Property Binding

The property binding is performed using json deserialization, therefore query parameter names can be customized using the `JsonPropertyName` attribute:

```csharp
app.MapGet("/test-query-model", (FromQuery<TestRecord> test) => test);

public record TestRecord(
    [property: JsonPropertyName("has-cake")] bool HasCake,
    [property: JsonPropertyName("eaten")]bool HasEatenIt
);
```

Example: `GET /test-query-model?has-cake=true&eaten=false`

### Data Annotations

If you prefer to use `System.ComponentModel.DataAnnotations` instead of `FluentValidation`, you can set the `FallbackToDataAnnotations` option to `true` when registering the validation services:

```csharp
// set the FallbackToDataAnnotations option to true
builder.Services.AddEndpointValidation<Program>(options =>
{
    options.FallbackToDataAnnotations = true;
});

// the FromQuery<T> model is validated automatically using the associated DataAnnotations
app.MapGet("/test-query-model", (FromQuery<TestRecord> test) => test);

public record TestRecord
{
    [Required]
    [MinLength(1)]
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [Required]
    [Range(1, 100)]
    [JsonPropertyName("age")]
    public int Age { get; set; }
}
```

Example: `GET /test-query-model?name=John&age=55`

### Explicit Validation

If you prefer to explicitly specify which arguments should be validated, you can set the `PreferExplicitRequestBodyValidation` option to `true` when registering the validation services, and use the `Validate<T>` endpoint filter instead:

```csharp
// set the PreferExplicitRequestBodyValidation option to true
builder.Services.AddEndpointValidation<Program>(options =>
{
    options.PreferExplicitRequestBodyValidation = true;
});

// endpoint with explicit validation
app.MapGet("/test-query-model", (FromQuery<TestRecord> test) => test)
    .Validate<TestRecord>();
```
