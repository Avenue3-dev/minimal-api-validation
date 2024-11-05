# How do I validate a request body?

You can validate any request body automatically by using the standard `[FromBody]` attribute and registering a [`FluentValidation`](https://fluentvalidation.net/) validator.

```csharp
// request body is validated automatically based on any registered FluentValidation validator
app.MapPost("/test-body", ([FromBody] TestRecord test) => { ... });

// request model
public record TestRecord(string Name, int Age);

// validator
public class TestRecordValidator : AbstractValidator<TestRecord>
{
    public TestRecordValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Age).InclusiveBetween(1, 100);
    }
}
```

As long as the associated validator is registered, the model will be automatically validated by the middleware. If validation fails, a `400 Bad Request` will be returned.

#### Data Annotations

If you prefer to use `System.ComponentModel.DataAnnotations` instead of `FluentValidation`, you can set the `FallbackToDataAnnotations` option to `true` when registering the validation services:

```csharp
// set the FallbackToDataAnnotations option to true
builder.Services.AddEndpointValidation<Program>(options =>
{
    options.FallbackToDataAnnotations = true;
});

// request body is validated automatically using the associated DataAnnotations
app.MapPost("/test-body", ([FromBody] TestRecord test) => { ... });

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

#### Explicit Validation

If you'd prefer not to automatically validate models marked with `[FromBody]`, and be explicit, you can set the `PreferExplicitRequestModelValidation` option to `true` when registering the validation services, and use the `Validate<T>` endpoint filter instead:

```csharp
// set the PreferExplicitRequestModelValidation option to true
builder.Services.AddEndpointValidation<Program>(options =>
{
    options.PreferExplicitRequestModelValidation = true;
});

// will no longer be validated
app.MapPost("/test-body", ([FromBody] TestRecord test) => { ... ]);

// will be validated
app.MapPost("/test-body", ([FromBody] TestRecord { ... }) => test).Validate<TestRecord>();

// collections are supported too
app.MapPost("/test-body", ([FromBody] TestRecord[] { ... }) => test).Validate<TestRecord>();
```
