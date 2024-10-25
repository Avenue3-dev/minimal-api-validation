namespace A3.MinimalApiValidation.Internal;

using FluentValidation.Results;
using Microsoft.Extensions.Logging;

internal static partial class LogMessages
{
    // information

    [LoggerMessage(LogLevel.Information, "Validating endpoint: {EndpointDisplayName}", EventId = 0)]
    internal static partial void Info_EndpointDetails(this ILogger logger, string endpointDisplayName);

    [LoggerMessage(LogLevel.Information, "Validation passed.", EventId = 0)]
    internal static partial void Info_ValidationPassed(this ILogger logger);

    [LoggerMessage(LogLevel.Information, "Validation failed with {ErrorCount} error(s):{@Errors}", EventId = 0)]
    internal static partial void Info_ValidationFailed(this ILogger logger, int errorCount, IEnumerable<ValidationFailure> errors);

    // debug

    [LoggerMessage(LogLevel.Debug, "Validating model of type {TypeName}", EventId = 0)]
    internal static partial void Debug_ValidatingModel(this ILogger logger, string typeName);

    [LoggerMessage(LogLevel.Debug, "Validating models array of type {TypeName}", EventId = 0)]
    internal static partial void Debug_ValidatingModelsArray(this ILogger logger, string typeName);

    [LoggerMessage(LogLevel.Debug, "No endpoint found.", EventId = 0)]
    internal static partial void Debug_NoEndpointFound(this ILogger logger);

    [LoggerMessage(LogLevel.Debug, "Reading endpoint metadata.", EventId = 0)]
    internal static partial void Debug_ReadingEndpointMetadata(this ILogger logger);

    [LoggerMessage(LogLevel.Debug, "Caching endpoint metadata.", EventId = 0)]
    internal static partial void Debug_CachingEndpointMetadata(this ILogger logger);

    [LoggerMessage(LogLevel.Debug, "No parameters found.", EventId = 0)]
    internal static partial void Debug_NoParametersFound(this ILogger logger);

    [LoggerMessage(LogLevel.Debug, "Checking {NumParams} parameter.", EventId = 0)]
    internal static partial void Debug_CheckingParameters(this ILogger logger, int numParams);

    [LoggerMessage(LogLevel.Debug, "Handling body parameter: {Name}.", EventId = 0)]
    internal static partial void Debug_HandlingBodyParameter(this ILogger logger, string name);

    [LoggerMessage(LogLevel.Debug, "Handling query parameter: {Name}.", EventId = 0)]
    internal static partial void Debug_HandlingQueryParameter(this ILogger logger, string name);

    [LoggerMessage(LogLevel.Debug, "Handling header parameter: {Name}.", EventId = 0)]
    internal static partial void Debug_HandlingHeaderParameter(this ILogger logger, string name);
}
