using EndpointValidator;
using EndpointValidator.Example.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddEndpointsApiExplorer()
    .AddEndpointValidation<Program>(options =>
    {
        options.FallbackToDataAnnotations = true;
    });

var app = builder.Build();

app
    .UseHttpsRedirection()
    .UseEndpointValidation()
    .RegisterEndpoints();

app.MapGet("/test", () => new {status = "test ok"});

app.Run();

namespace EndpointValidator.Example
{
    public class Program;
}
