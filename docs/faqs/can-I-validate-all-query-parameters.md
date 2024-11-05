# Can I validate all query parameters together?

Yes! You can use the custom binder `FromQuery<T>` to bind a subset, or all query parameters into a single model. This model can then be validated in the same way as a body request model using `FluentValidation` or data annotations:

To validate, just register a FluentValidation validator for the same type.

```csharp
app.MapGet("/test-query-model", (FromQuery<TestRecord> test) => { ... });

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

See the [`FromQuery<T>` docs](../custom-binders/FromQuery.md) for more information.
