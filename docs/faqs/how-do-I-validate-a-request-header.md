# How do I validate a request header?

You can validate request headers using the standard `[FromHeader]` attribute in combination with any `ValidationAttribute`:

```csharp
app.MapGet("/test-header", ([FromHeader, MinLength(1)] string header) => { ... });
```

#### Example Usage

```csharp
// header is required, a bad request will be returned if it is missing
[FromHeader] int testHeader

// header is optional
[FromHeader(Name = "x-test")] int? testHeader

// header is required and must be between 1 and 5 inclusive
[FromHeader(Name = "x-test"), Range(1, 5)] int testHeader

// header is optional, but if supplied, it must be between 1 and 5 inclusive
[FromHeader(Name = "x-test"), Range(1, 5)] int? testHeader
```
