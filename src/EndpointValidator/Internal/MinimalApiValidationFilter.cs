namespace EndpointValidator.Internal;

using System.Collections.Concurrent;
using System.Reflection;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

internal class MinimalApiValidationFilter : IMiddleware
{
    private static ConcurrentDictionary<string, ParameterAttributeInfo[]> EndpointParameterCache { get; } = new();

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var options = context.RequestServices.GetService<MinimalApiValidationOptions>()
            ?? new MinimalApiValidationOptions();

        var endpoint = context.GetEndpoint();
        if (endpoint is null)
        {
            await next(context);
            return;
        }

        if (!EndpointParameterCache.TryGetValue(endpoint.DisplayName ?? "", out var args))
        {
            args = endpoint.Metadata
                .OfType<MethodInfo>()
                .FirstOrDefault()?
                .GetParameters()
                .Select(x => new ParameterAttributeInfo(x))
                .Where(x => x.IsBody || x.IsQuery || x.IsHeader)
                .ToArray() ?? [];

            if (endpoint.DisplayName is not null)
            {
                EndpointParameterCache.TryAdd(endpoint.DisplayName, args);
            }
        }

        if (args.Length == 0)
        {
            await next(context);
            return;
        }

        var errors = new List<ValidationFailure>();

        foreach (var arg in args)
        {
            if (arg.IsBody)
            {
                errors.AddRange(await RequestBody.HandleAsync(arg, context, options));
            }
            else if (arg.IsQuery)
            {
                errors.AddRange(HeaderOrQuery.Handle(arg, context));
            }
            else if (arg.IsHeader)
            {
                errors.AddRange(HeaderOrQuery.Handle(arg, context));
            }
        }

        if (errors.Count == 0)
        {
            await next(context);
            return;
        }

        var result = new ValidationResult(errors).ToDictionary();
        await Results.ValidationProblem(result).ExecuteAsync(context);
    }
}
