namespace EndpointValidator.Example.Endpoints;

using System.Reflection;

public interface IEndpoint
{
    static abstract void Add(IEndpointRouteBuilder app);
}

internal static class EndpointRegistration
{
    public static IApplicationBuilder RegisterEndpoints(this IApplicationBuilder app)
    {
        var endpoints = typeof(Program).Assembly
            .GetTypes()
            .Where(t => t.IsClass && t is {IsAbstract: false, IsPublic: true})
            .Where(t => t.GetInterfaces().Any(x => x.IsAssignableFrom(typeof(IEndpoint))))
            .ToList();

        endpoints.ForEach(x =>
        {
            x.GetMethod(nameof(IEndpoint.Add), BindingFlags.Public | BindingFlags.Static)?.Invoke(null, [app]);
        });

        return app;
    }
}
