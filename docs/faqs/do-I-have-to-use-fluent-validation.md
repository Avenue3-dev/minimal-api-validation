# Do I have to use FluentValidation?

No.

Whilst we recommend [`FluentValidation`](https://fluentvalidation.net/) as a validation library, you can use `System.ComponentModel.DataAnnotations` instead:

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
    public string Name { get; set; } = "";

    [Required]
    [Range(1, 100)]
    public int Age { get; set; }
}
```
