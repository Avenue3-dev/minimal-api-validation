# Can I turn off the automatic validation and be explicit about what to validate?

If you'd prefer not to automatically validate models marked with `[FromBody]`, and be explicit, you can set the `PreferExplicitRequestModelValidation` option to `true` when registering the validation services, and use the `Validate<T>` endpoint filter instead:

```csharp
// set the PreferExplicitRequestModelValidation option to true
builder.Services.AddEndpointValidation<Program>(options =>
{
    options.PreferExplicitRequestModelValidation = true;
});

// will no longer be validated
app.MapPost("/test-body", ([FromBody] TestRecord test) => { ... });

// will be validated
app.MapPost("/test-body", ([FromBody] TestRecord test) => { ... }).Validate<TestRecord>();

// arrays are supported too
app.MapPost("/test-body", ([FromBody] TestRecord[] tests) => { ... }).Validate<TestRecord>();
```
