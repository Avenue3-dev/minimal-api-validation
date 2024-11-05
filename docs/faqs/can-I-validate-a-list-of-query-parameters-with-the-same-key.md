# Can I validate a list of query parameters with the same key?

Yes! You can bind all query parameters with the same key into a list (providing they are of the same type) using the `FromQuery<T>` binder:

```csharp
app.MapGet("/test-query-model", FromQuery<MyModel> model) => { ... });
```

The model you map to will then be automatically validated following the same conventions as body request models:

```csharp
app.MapGet("/test-query-model", (FromQuery<IdList> Ids) => { ... });

public record IdList(
    [property: FromQuery(Name = "id")]int[] Ids
);

public class IdListValidator : AbstractValidator<IdList>
{
    public IdListValidator()
    {
        RuleForEach(x => x.Ids)
            .GreaterThan(0)
            .WithMessage("Each provided id must be greater than 0");
    }
}
```

## Partial Mapping and Multiple Lists

Multiple lists are supported, as is just matching a subset of the query parameters, for example:

```csharp
app.MapGet("/test-query-model", ([FromQuery] string name, FromQuery<TestRecord> test) => { ... });

public record TestRecord(
    Guid Reference,
    [property: FromQuery(Name = "id")]int[] Ids
    [property: FromQuery(Name = "item")]string[] Items
);

public class TestRecordValidator : AbstractValidator<TestRecord>
{
    public TestRecordValidator()
    {
        RuleFor(x => x.Reference).NotEmpty();
        RuleForEach(x => x.Ids).GreaterThan(0);
        RuleForEach(x => x.Items).NotEmpty();
    }
}
```
Example:
```
GET /test-query-model
  ?name=John
  &items=apple
  &items=banana
  &ids=1
  &ids=2
  &ids=3
  &reference=8419402A-006F-4624-8764-2392E9C09DF3
```
