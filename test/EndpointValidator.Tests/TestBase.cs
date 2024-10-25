namespace EndpointValidator.Tests;

using EndpointValidator.Tests.Client;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

public abstract class TestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected TestBase(WebApplicationFactory<Program> factory)
    {
        Client = factory.WithWebHostBuilder(builder =>
        {
            builder
                .ConfigureServices(svc =>
                {
                    if (RegisterValidator)
                    {
                        svc.AddSingleton<IValidator<TestRecord>, TestRecordValidator>();
                    }
                    svc.AddEndpointValidation(ConfigureOptions);
                })
                .Configure(app =>
                {
                    app
                        .UseRouting()
                        .UseEndpointValidation()
                        .UseEndpoints(AddTestEndpoint);
                });
        }).CreateClient();
    }

    protected HttpClient Client { get; }

    protected string Path { get; } = Guid.NewGuid().ToString();

    protected abstract void AddTestEndpoint(IEndpointRouteBuilder app);

    protected virtual bool RegisterValidator => true;

    protected virtual void ConfigureOptions(EndpointValidatorOptions options)
    {
    }
}
