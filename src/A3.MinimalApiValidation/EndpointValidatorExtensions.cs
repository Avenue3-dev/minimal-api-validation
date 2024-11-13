namespace A3.MinimalApiValidation;

using A3.MinimalApiValidation.Internal.Filter;
using A3.MinimalApiValidation.Internal.Middleware;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

public static class EndpointValidationExtensions
{
    /// <summary>
    /// Registers the MinimalApiValidation middleware with the provided options and
    /// registers all validators from the assembly containing the specified type.
    /// <para>
    /// The validators are registered using the AddValidatorsFromAssemblyContaining{T}
    /// extension method from FluentValidation.DependencyInjectionExtensions. If you wish
    /// to configure this yourself, use the <see cref="AddEndpointValidation"/> extension method
    /// instead.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">An optional action that can be used to configure options of the validation middleware.</param>
    /// <typeparam name="T">A type that belongs to an assembly containing the validators you wish to register.</typeparam>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddEndpointValidation<T>(
        this IServiceCollection services,
        Action<EndpointValidatorOptions>? configure = null) where T : class
    {
        services
            .AddEndpointValidation(configure)
            .AddValidatorsFromAssemblyContaining<T>()
            ;

        return services;
    }

    /// <summary>
    /// Registers the MinimalApiValidation middleware with the provided options.
    /// <para>
    /// You must also register validators in the DI container for the types you want to validate.
    /// Alternatively, you can use the <see cref="AddEndpointValidation{T}"/> extension method.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">An optional action that can be used to configure options of the validation middleware.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddEndpointValidation(
        this IServiceCollection services,
        Action<EndpointValidatorOptions>? configure = null)
    {
        var options = new EndpointValidatorOptions();
        configure?.Invoke(options);

        services
            .AddSingleton(options)
            .AddTransient<ValidationMiddleware>()
            .AddHttpContextAccessor()
            ;

        return services;
    }

    /// <summary>
    /// Adds the MinimalApiValidation middleware to the application pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseEndpointValidation(this IApplicationBuilder app)
    {
        app
            .UseMiddleware<ValidationMiddleware>()
            ;

        return app;
    }

    /// <summary>
    /// Adds an endpoint filter to automatically validate a model type using
    /// FluentValidation and return a validation error.
    /// <para>
    /// An <see cref="IValidator"/> must be registered in the DI container for the model type.
    /// </para>
    /// <para>
    /// This filter will be ignored unless the <see cref="EndpointValidatorOptions.PreferExplicitRequestModelValidation"/>
    /// is set to <c>true</c>.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type to validate with a registered FluentValidation <see cref="IValidator{T}"/>.</typeparam>
    /// <param name="builder">The route handler builder.</param>
    /// <returns>The route handler builder.</returns>
    public static RouteHandlerBuilder Validate<T>(this RouteHandlerBuilder builder)
        where T : class
    {
        builder
            .AddEndpointFilter<RequestModelValidationFilter<T>>()
            .ProducesValidationProblem();

        return builder;
    }

    /// <summary>
    /// Adds an endpoint filter that will allow validation of the request, but not
    /// execute the endpoint if the validate only header is present.
    /// <para>A 202 Accepted response is returned if the request is valid.</para>
    /// <para>The default header is `x-validate-only`, but can be overridden in the <see cref="EndpointValidatorOptions"/>.</para>
    /// </summary>
    /// <param name="builder">The route handler builder.</param>
    /// <returns>The route handler builder.</returns>
    public static RouteHandlerBuilder WithValidateOnly(this RouteHandlerBuilder builder)
    {
        builder
            .AddEndpointFilter<ValidateOnlyFilter>()
            .Produces<ValidateOnlyResponse>(
                statusCode: StatusCodes.Status202Accepted,
                contentType: "application/json");

        return builder;
    }
}
