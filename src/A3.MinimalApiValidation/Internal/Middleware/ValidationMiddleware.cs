namespace A3.MinimalApiValidation.Internal.Middleware;

using System.Collections.Concurrent;
using System.Reflection;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

internal class ValidationMiddleware : IMiddleware
{
    private static ConcurrentDictionary<string, ParameterAttributeInfo[]> EndpointParameterCache { get; } = new();

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var options = context.RequestServices.GetService<EndpointValidatorOptions>()
            ?? EndpointValidatorOptions.Default;

        if (options.PreferExplicitRequestBodyValidation)
        {
            await next(context);
            return;
        }

        var logger = context.GetLogger<ValidationMiddleware>();

        var endpoint = context.GetEndpoint();
        if (endpoint is null)
        {
            logger.Debug_NoEndpointFound();

            await next(context);
            return;
        }

        logger.Info_EndpointDetails(endpoint.DisplayName ?? "[NO DISPLAY NAME]");

        if (!EndpointParameterCache.TryGetValue(endpoint.DisplayName ?? "", out var args))
        {
            logger.Debug_ReadingEndpointMetadata();

            args = endpoint.Metadata
                .OfType<MethodInfo>()
                .FirstOrDefault()?
                .GetParameters()
                .Select(x => new ParameterAttributeInfo(x))
                .Where(x => x.IsBody || x.IsQuery || x.IsHeader)
                .ToArray() ?? [];

            if (endpoint.DisplayName is not null)
            {
                logger.Debug_CachingEndpointMetadata();
                EndpointParameterCache.TryAdd(endpoint.DisplayName, args);
            }
        }

        if (args.Length == 0)
        {
            logger.Debug_NoParametersFound();

            await next(context);
            return;
        }

        var errors = new List<ValidationFailure>();
        string? detail = null;

        logger.Debug_CheckingParameters(args.Length);
        foreach (var arg in args)
        {
            if (arg.IsBody && !options.PreferExplicitRequestBodyValidation)
            {
                logger.Debug_HandlingBodyParameter(arg.Name);

                var bodyResult = await Body.HandleAsync(arg, context, options);

                errors.AddRange(bodyResult.Errors);
                detail = bodyResult.Detail;
            }
            else if (arg.IsQuery)
            {
                logger.Debug_HandlingQueryParameter(arg.Name);
                errors.AddRange(HeaderOrQuery.Handle(arg, context));
            }
            else if (arg.IsHeader)
            {
                logger.Debug_HandlingHeaderParameter(arg.Name);
                errors.AddRange(HeaderOrQuery.Handle(arg, context));
            }
        }

        if (errors.Count == 0)
        {
            logger.Info_ValidationPassed();

            await next(context);
            return;
        }

        var result = new ValidationResult(errors).ToDictionary();

        logger.Info_ValidationFailed(errors.Count, errors);

        await Results.ValidationProblem(result, detail: detail).ExecuteAsync(context);
    }
}
