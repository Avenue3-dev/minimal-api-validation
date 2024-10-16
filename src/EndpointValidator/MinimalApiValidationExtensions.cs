namespace EndpointValidator;

using EndpointValidator.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public static class MinimalApiValidationExtensions
{
    /// <summary>
    /// Registers the MinimalApiValidation middleware with the provided options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">An optional action that can be used to configure options of the validation middleware.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddMinimalApiValidation(
        this IServiceCollection services,
        Action<MinimalApiValidationOptions>? configure = null)
    {
        var options = new MinimalApiValidationOptions();
        configure?.Invoke(options);

        services
            .AddSingleton(options)
            .AddTransient<MinimalApiValidationFilter>()
            ;

        return services;
    }

    /// <summary>
    /// Adds the MinimalApiValidation middleware to the application pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseMinimalApiValidation(this IApplicationBuilder app)
    {
        app
            .UseMiddleware<MinimalApiValidationFilter>()
            ;

        return app;
    }
}

public class MinimalApiValidationOptions
{
    /// <summary>
    /// If set to true, the validation will fallback to using DataAnnotations if no FluentValidation validators are found.
    /// <para>The default value is <c>flase</c></para>
    /// </summary>
    public bool FallbackToDataAnnotations { get; set; }

    /// <summary>
    /// Use to provide custom JsonSerializerOptions for the deserialization of the request body. If not provided, the options
    /// will be resolved from the DI container using <see cref="Microsoft.AspNetCore.Http.Json.JsonOptions"/> followed by
    /// <see cref="IOptions{TOptions}"/>.
    /// <para>The default value is <c>null</c></para> 
    /// </summary>
    public System.Text.Json.JsonSerializerOptions? JsonSerializerOptions { get; set; } = null;
}
