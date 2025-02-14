# MinimalApiValidation

[![build](https://github.com/Avenue3-dev/minimal-api-validation/actions/workflows/main.yml/badge.svg)](https://github.com/Avenue3-dev/minimal-api-validation/actions/workflows/main.yml) [![nuget](https://img.shields.io/nuget/v/Avenue3.MinimalApiValidation.svg?style=flat)](https://www.nuget.org/packages/Avenue3.MinimalApiValidation) ![license](https://img.shields.io/github/license/Avenue3-dev/eslint-config-avenue3)

Easily add validation to your ASP.NET Core Minimal API endpoints to validate incoming request headers, query parameters, and the request body.

- Use `[FromBody]` to automatically validate arguments using any registered `FluentValidation` validator.
- Use `[FromHeader]` in combination with any `ValidationAttribute` to automatically validate header arguments.
- Use `[FromQuery]` in combination with any `ValidationAttribute` to automatically validate header query parameters.
- Use custom `FromQuery<T>` to automatically bind and validate a group of dependent query parameters.

## Installation

To install the package:

```bash
dotnet add package Avenue3.MinimalApiValidation
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

> Note: <br /> If you plan to use `FluentValidation`, make sure to register your validators with the service collection, or use `AddEndpointValidation<T>()` to automatically register all validators found in the assembly containing `T`.

```csharp
// add validation services and default options (you will need to register your FluentValidation validators)
builder.Services.AddEndpointValidation();

// OR

// automatically register all FluentValidation validators in the assembly that contains the specified type
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

#### Fallback To Data Annotations

By default, `[FromBody]` validation is done using any registered `FluentValidation` validator. If you prefer to use `System.ComponentModel.DataAnnotations` or you have a mixture of the two, you can set this option to `true` to fallback to use `DataAnnotations` when no `FluentValidation` validator is found.

```csharp
builder.Services.AddEndpointValidation<Program>(options =>
{
    // the default value is false
    options.FallbackToDataAnnotations = true;
});
```

#### Prefer Explicit Request Model Validation

By default, validation is performed automatically (implicitly) for all `[FromBody]` arguments. If you would prefer to explicitly specify which arguments should be validated, you can set this option to `true`, and use the `Validate<T>` endpoint filter instead:

```csharp
builder.Services.AddEndpointValidation<Program>(options =>
{
    // the default value is false
    options.PreferExplicitRequestModelValidation = true;
});

// will not be validated
app.MapPost("/test-body", ([FromBody] TestRecord test) => test)

// will be validated
app.MapPost("/test-body", ([FromBody] TestRecord test) => test).Validate<TestRecord>();
```

#### Json Serializer Options

This sets the `JsonSerializerOptions` used by the validation middleware. By default, the middleware will try to resolve the serializer options from the service collection via `Microsoft.AspNetCore.Http.Json.JsonOptions`:

```csharp
builder.Services.AddEndpointValidation<Program>(options =>
{
    // the default value is null
    options.JsonSerializerOptions = new System.Text.Json.JsonSerializerOptions { ... };
});
```

#### Validate Only Header

This sets the header that the middleware will look for to determine if the request should only be validated. If the header is present, the request will be validated, but the endpoint will not be executed:

```csharp
builder.Services.AddEndpointValidation<Program>(options =>
{
    // the default value is "x-validate-only"
    options.ValidateOnlyHeader = "x-custom-validate-only";
});
```

> See the [Validation Only](#validation-only) section for more information.

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
[FromHeader(Name = "x-test"), Range(1, 5)] int testHeader

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

#### Map multiple query parameters to a single object

You can also map multiple query parameters to a single object using the `FromQuery<T>` binder. This allows you to bind and validate a group of dependent query parameters:

```csharp
// query parameters are matched to the given object and validated automatically based on any registered FluentValidation validator
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

// query parameters
GET /test-query-model?name=John&hasCake=true&hasEatenIt=false
```

> You can also use the same validation options as with `FromBody` (see below), including using DataAnnotations instead of FluentValidation, and explicit validation rather than automatic validation.

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

If you prefer to explicitly specify which arguments should be validated, you can set the `PreferExplicitRequestModelValidation` option to `true` when registering the validation services, and use the `Validate<T>` endpoint filter instead:

```csharp
// set the PreferExplicitRequestModelValidation option to true
builder.Services.AddEndpointValidation<Program>(options =>
{
    options.PreferExplicitRequestModelValidation = true;
});

// endpoint with explicit validation
app.MapPost("/test-body", ([FromBody] TestRecord test) => test)
    .Validate<TestRecord>();

// collections are supported too
app.MapPost("/test-body", ([FromBody] TestRecord[] tests) => test)
    .Validate<TestRecord>();
```

## Validation Attributes

You can use any `ValidationAttribute` to validate headers, query parameters, or body parameter (with the correct configuration). This means you can use any of the built-in attributes or create your own custom attributes, for example:

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

// query parameter is optional, but must be greater than or equal to 1 when provided
[FromQuery, Min(1)] int? page
```

### Custom Validation Attributes

This package contains a number of custom validation attributes. You can find the available attributes and documentation in the GitHub repo [docs](https://github.com/Avenue3-dev/minimal-api-validation/tree/main/docs/validation-attributes) folder.

## Validation Results

If validation fails, a `ValidationProblem` result is returned which produces a `400 Bad Request` response with a `HttpValidationProblemDetails` JSON payload. The payload contains a dictionary of errors where the key is the argument name and the value is a list of validation errors:

```json
{
  "errors": {
    "Name": ["The Name field is required."],
    "Age": ["The Age field must be between 1 and 100."]
  }
}
```

## Validation Only

If you want to validate a request without executing the endpoint, you can use the `WithValidateOnly` endpoint filter in combination with the optional `ValidateOnlyHeader`:

```csharp
app.MapPost("/test-body", ([FromBody] TestRecord test) => test).WithValidateOnly();
```

This filter looks for the configured `ValidateOnlyHeader` in the request and returns and intercepts the request between validation and endpoint execution, returning a `202 Accepted` response if the request is valid.

> The header is optional, and must be present and set to `true` for the filter to take effect.

Request:
```http request
POST /test-body
Content-Type: application/json
x-validate-only: true

{
  "name": "John",
  "age": 30
}
```

Response:
```json
{
  "isValid": true,
  "message": "Request was validated but not processed.",
  "timestamp": "2024-11-12T14:30:26.3682090+00:00"
}
```

### Custom Validate Only Header

The default header is `x-validate-only`, but this can be configured when registering the validation services:

```csharp
builder.Services.AddEndpointValidation<Program>(options =>
{
    options.ValidateOnlyHeader = "x-custom-validate-only";
});
```

## FAQs

You can check out more docs and some FAQs [here](./docs)

## Examples

You can find some examples in the [examples](./examples) folder.
