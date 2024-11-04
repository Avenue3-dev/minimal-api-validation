# How do I register the validation middleware?

Register the validation middleware in your `Program.cs`:

```csharp
using A3.MinimalApiValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointValidation(); // <--- Register the middleware

var app = builder.Build();

app.UseEndpointValidation(); // <--- Use the middleware

app.MapGet("/test-query", ([FromQuery, Range(1, 5)] int page) => new { page });

app.Run();
```

## Service Configuration
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

> Note: <br /> If you plan to use `FluentValidation`, make sure to register your validators with the service collection, or use `AddEndpointValidation<T>()` to automatically register all validators found in the assembly containing `T`.
 
