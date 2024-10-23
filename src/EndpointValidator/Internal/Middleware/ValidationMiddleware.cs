namespace EndpointValidator.Internal.Middleware;

using System.Collections.Concurrent;
using System.Reflection;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

internal class ValidationMiddleware : IMiddleware
{
    private static ConcurrentDictionary<string, ParameterAttributeInfo[]> EndpointParameterCache { get; } = new();

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var options = context.RequestServices.GetService<EndpointValidatorOptions>()
            ?? Utils.DefaultOptions;

        var logger = (context.RequestServices.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance)
            .CreateLogger(options.LoggerCategoryName);

        var endpoint = context.GetEndpoint();
        if (endpoint is null)
        {
            logger.LogDebug("No endpoint found.");

            await next(context);
            return;
        }

        logger.LogDebug("Endpoint: {EndpointDisplayName}.", endpoint.DisplayName ?? "[NO DISPLAY NAME]");

        if (!EndpointParameterCache.TryGetValue(endpoint.DisplayName ?? "", out var args))
        {
            logger.LogDebug("Reading endpoint metadata.");

            args = endpoint.Metadata
                .OfType<MethodInfo>()
                .FirstOrDefault()?
                .GetParameters()
                .Select(x => new ParameterAttributeInfo(x))
                .Where(x => x.IsBody || x.IsQuery || x.IsHeader)
                .ToArray() ?? [];

            if (endpoint.DisplayName is not null)
            {
                logger.LogDebug("Caching endpoint metadata.");
                EndpointParameterCache.TryAdd(endpoint.DisplayName, args);
            }
        }

        if (args.Length == 0)
        {
            logger.LogDebug("No parameters found.");

            await next(context);
            return;
        }

        var errors = new List<ValidationFailure>();
        string? detail = null;

        logger.LogInformation("Validating endpoint: {EndpointDisplayName}.", endpoint.DisplayName ?? "[NO DISPLAY NAME]");
        logger.LogDebug("Checking {numParams} parameter.", args.Length);
        foreach (var arg in args)
        {
            if (arg.IsBody && !options.PreferExplicitRequestBodyValidation)
            {
                logger.LogDebug("Handling body parameter: {name}.", arg.Name);
                
                var bodyResult = await Body.HandleAsync(arg, context, options);
                
                errors.AddRange(bodyResult.Errors);
                detail = bodyResult.Detail;
            }
            else if (arg.IsQuery)
            {
                logger.LogDebug("Handling query parameter: {name}.", arg.Name);
                errors.AddRange(HeaderOrQuery.Handle(arg, context));
            }
            else if (arg.IsHeader)
            {
                logger.LogDebug("Handling header parameter: {name}.", arg.Name);
                errors.AddRange(HeaderOrQuery.Handle(arg, context));
            }
        }

        if (errors.Count == 0)
        {
            logger.LogInformation("Validation passed.");

            await next(context);
            return;
        }

        var result = new ValidationResult(errors).ToDictionary();

        logger.LogWarning(
            "Validation failed with {numErrors} error(s):{newline}{errors}",
            errors.Count,
            Environment.NewLine,
            string.Join(Environment.NewLine, errors.Select(e => $"- {e.PropertyName}: {e.ErrorMessage}"))
        );

        await Results.ValidationProblem(result, detail: detail).ExecuteAsync(context);
    }
}
