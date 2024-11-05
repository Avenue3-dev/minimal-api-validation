# How do I validate a query parameter?

You can validate query parameters using the standard `[FromQuery]` attribute in combination with any `ValidationAttribute`:

```csharp
app.MapGet("/test-query", ([FromQuery, Range(1, 5)] int page) => new { ... });
```

## Example Usage

```csharp
// query parameter is required, a bad request will be returned if it is missing
[FromQuery] string filterText

// query parameter is optional
[FromQuery(Name = "filter")] string? filterText

// query parameter is required and must be at least 1 character long
[FromQuery, MinLength(1)] string filter

// query parameter is optional, but if supplied, it must be at no more than 20 characters long
[FromQuery, MaxLength(20)] string? page
```
