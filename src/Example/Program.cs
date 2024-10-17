using A3.MinimalApiValidation;
using Example.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddEndpointValidation<Program>(options =>
    {
        options.FallbackToDataAnnotations = true;
    });

var app = builder.Build();

app
    .UseHttpsRedirection()
    .UseEndpointValidation()
    .RegisterEndpoints();

app.Run();
