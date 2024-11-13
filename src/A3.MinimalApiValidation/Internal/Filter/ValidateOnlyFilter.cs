namespace A3.MinimalApiValidation.Internal.Filter;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

internal class ValidateOnlyFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var timestamp = context.HttpContext.RequestServices.GetService<TimeProvider>()?.GetUtcNow() ?? DateTime.UtcNow;
        
        var options = context.HttpContext.RequestServices.GetService<EndpointValidatorOptions>()
            ?? EndpointValidatorOptions.Default;

        var logger = context.HttpContext.GetLogger<ValidateOnlyFilter>();
        
        if (context.HttpContext.Request.Headers.TryGetValue(options.ValidateOnlyHeader, out var validateOnly)
            && validateOnly.ToString().Equals("true", StringComparison.CurrentCultureIgnoreCase))
        {
            logger.Debug_ValidateOnlyHeaderSet();
            
            return Results.Accepted(null, new ValidateOnlyResponse
            (
                IsValid: true,
                Message: "Request was validated but not processed.",
                Timestamp: timestamp.ToString("O")
            ));
        }

        return await next(context);
    }
}

public record ValidateOnlyResponse(bool IsValid, string Message, string Timestamp)
{
    /// <summary>
    /// A value indicating whether or not the request passed validation.
    /// </summary>
    /// <example>true</example>
    public bool IsValid { get; } = IsValid;

    /// <summary>
    /// A message indicating that the result was validated but not processed.
    /// </summary>
    /// <example>Request was validated but not processed.</example>
    public string Message { get; } = Message;

    /// <summary>
    /// UTC timestamp of when the request was validated.
    /// </summary>
    /// <example>2024-07-04T08:45:42.9763790Z</example>
    public string Timestamp { get; } = Timestamp;
}
