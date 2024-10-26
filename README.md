# MinimalApiValidation

// TODO: Add Badges

Easily add validation to your ASP.NET Core Minimal API endpoints to validate incoming request headers, query parameters, and the request body.

- Use `[FromBody]` to automatically validate arguments using any registered `FluentValidation` validator.
- Use `[FromHeader]` in combination with any `ValidationAttribute` to automatically validate header arguments.
- Use `[FromQuery]` in combination with any `ValidationAttribute` to automatically validate header query parameters.

## Installation

To install the package:

```bash
dotnet add package A3.MinimalApiValidation
```

## Basic Usage

```csharp
var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddEndpointValidation<Program>();

var app = builder.Build();

app.UseEndpointValidation();

// headers and query parameters are validated automatically using any ValidationAttribute
app.MapGet("/test-header", ([FromHeader, MinLength(1)] string header) => new { header });
app.MapGet("/test-query", ([FromQuery, Range(1, 5)] int page) => new { page });

// request body is validated automatically using any registered FluentValidation validator
app.MapPost("/test-body", ([FromBody] TestRecord test) => test);

app.Run();

public record TestRecord(string Name, int Age);

public class TestRecordValidator : AbstractValidator<TestRecord>
{
    public TestRecordValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Age).InclusiveBetween(1, 100);
    }
}
```

## Registration and Options

To use the validation features, you need to register the validation services and options in the `WebApplicationBuilder`:

> Note: <br /> If you plan to use `FluentValidation`, make sure to register your validators with the service collection, or use the `AddEndpointValidation<T>`.

```csharp
// add validation services and default options (you will need to register your validators)
builder.Services.AddEndpointValidation();

// OR

// add validation services, default options, and register all validators in the assembly that contains the specified type
builder.Services.AddEndpointValidation<Program>();

// OR

// you can override the default options with either method
builder.Services.AddEndpointValidation(options => { ... });
builder.Services.AddEndpointValidation<Program>(options => { ... });
```

Then, add the middleware to the request pipeline:

```csharp
app.UseEndpointValidation();
```

### Options

Several options can be configured when registering the validation services:

#### FallbackToDataAnnotations

By default, `[FromBody]` validation is done using any registered `FluentValidation` validator. If you prefer to use `System.ComponentModel.DataAnnotations` or you have a mixture of the two, you can set this option to `true` to fallback to use `DataAnnotations` when no `FluentValidation` validator is found.

```csharp
builder.Services.AddEndpointValidation<Program>(options =>
{
    // the default value is false
    options.FallbackToDataAnnotations = true;
});
```

#### PreferExplicitRequestBodyValidation

By default, validation is performed automatically (implicitly) for all `[FromBody]` arguments. If you would prefer to explicitly specify which arguments should be validated, you can set this option to `true`, and use the `Validate<T>` endpoint filter instead:

```csharp
builder.Services.AddEndpointValidation<Program>(options =>
{
    // the default value is false
    options.PreferExplicitRequestBodyValidation = true;
});

// endpoint with explicit validation
app.MapPost("/test-body", ([FromBody] TestRecord test) => test)
    .Validate<TestRecord>();
```

#### LoggerCategoryName

This sets the logger category name used by the validation middleware.

```csharp
builder.Services.AddEndpointValidation<Program>(options =>
{
    // the default value is "MinimalApiValidation"
    options.LoggerCategoryName = "Custom.MinimalApiValidation.Category";
});
```

#### JsonSerializerOptions

This sets the `JsonSerializerOptions` used by the validation middleware. By default, the middleware will try to resolve the serializer options from the service collection via `Microsoft.AspNetCore.Http.Json.JsonOptions`:

```csharp
builder.Services.AddEndpointValidation<Program>(options =>
{
    // the default value is null
    options.JsonSerializerOptions = new System.Text.Json.JsonSerializerOptions { ... };
});
```

## Validation

The package provides several features and helpers for validating incoming requests:

### Request Headers

You can validate request headers using the `[FromHeader]` attribute in combination with any `ValidationAttribute`:

```csharp
app.MapGet("/test-header", ([FromHeader, MinLength(1)] string header) => new { header });
```

#### Example Usage

```csharp
// header is required, a bad request will be returned if it is missing
[FromHeader] int testHeader

// header is optional
[FromHeader(Name = "x-test")] int? testHeader

// header is required and must be between 1 and 5 inclusive
[FromHeader(Name = "x-test"), Rnage(1, 5)] int testHeader

// header is optional, but if supplied, it must be between 1 and 5 inclusive
[FromHeader(Name = "x-test"), Range(1, 5)] int? testHeader
```

### Query Parameters

You can validate query parameters using the `[FromQuery]` attribute in combination with any `ValidationAttribute`:

```csharp
app.MapGet("/test-query", ([FromQuery, Range(1, 5)] int page) => new { page });
```

#### Example Usage

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

### Request Body

You can validate any request body automatically by using the `[FromBody]` attribute and registering a `FluentValidation` validator.

```csharp
// request body is validated automatically based on any registered FluentValidation validator
app.MapPost("/test-body", ([FromBody] TestRecord test) => test);

public record TestRecord(string Name, int Age);

public class TestRecordValidator : AbstractValidator<TestRecord>
{
    public TestRecordValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Age).InclusiveBetween(1, 100);
    }
}
```

#### Data Annotations

If you prefer to use `System.ComponentModel.DataAnnotations` instead of `FluentValidation`, you can set the `FallbackToDataAnnotations` option to `true` when registering the validation services:

```csharp
// set the FallbackToDataAnnotations option to true
builder.Services.AddEndpointValidation<Program>(options =>
{
    options.FallbackToDataAnnotations = true;
});

// request body is validated automatically using the associated DataAnnotations
app.MapPost("/test-body", ([FromBody] TestRecord test) => test);

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

If you prefer to explicitly specify which arguments should be validated, you can set the `PreferExplicitRequestBodyValidation` option to `true` when registering the validation services, and use the `Validate<T>` endpoint filter instead:

```csharp
// set the PreferExplicitRequestBodyValidation option to true
builder.Services.AddEndpointValidation<Program>(options =>
{
    options.PreferExplicitRequestBodyValidation = true;
});

// endpoint with explicit validation
app.MapPost("/test-body", ([FromBody] TestRecord test) => test)
    .Validate<TestRecord>();

// arrays are supported
app.MapPost("/test-body", ([FromBody] TestRecord[] tests) => test)
    .Validate<TestRecord>();
```

### Validation Attributes

You can use any `ValidationAttribute` to validate headers, query parameters. This means you can use any of the built-in attributes or create your own custom attributes:

```csharp
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class MinAttribute : ValidationAttribute
{
    public MinAttribute(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public override bool IsValid(object? value) => value is int i && i >= Value;

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be greater than or equal to {Value}";
    }
}
```

#### Example Usage

```csharp
// header is required and must be greater than or equal to 1
[FromHeader(Name = "x-test"), Min(1)] int testHeader

// query parameter is required and must be greater than or equal to 1
[FromQuery, Min(1)] int page
```

### Validation Results

If validation fails, a `ValidationProblem` result is returned which produces a `400 Bad Request` response with a `HttpValidationProblemDetails` JSON payload. The payload contains a dictionary of errors where the key is the argument name and the value is a list of validation errors:

```json
{
  "errors": {
    "Name": ["The Name field is required."],
    "Age": ["The Age field must be between 1 and 100."]
  }
}
```
