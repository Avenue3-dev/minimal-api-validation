namespace EndpointValidator;

using EndpointValidator.Internal.Filter;
using Microsoft.Extensions.Options;

public class EndpointValidatorOptions
{
    /// <summary>
    /// If set to <c>true</c>, the validation will fallback to using DataAnnotations
    /// if no FluentValidation validators are found.
    /// <para>The default value is <c>false</c></para>
    /// </summary>
    public bool FallbackToDataAnnotations { get; set; }

    /// <summary>
    /// Use to provide custom JsonSerializerOptions for the deserialization of
    /// the request body. If not provided, the options will be resolved from the
    /// DI container using <see cref="Microsoft.AspNetCore.Http.Json.JsonOptions"/>
    /// followed by <see cref="IOptions{TOptions}"/>.
    /// <para>The default value is <c>null</c></para> 
    /// </summary>
    public System.Text.Json.JsonSerializerOptions? JsonSerializerOptions { get; set; } = null;

    /// <summary>
    /// If set to <c>true</c>, request body validation will only be performed if
    /// the endpoint uses the <see cref="RequestBodyValidationFilter{T}" /> endpoint filter.
    /// <para>The default value is <c>false</c></para>
    /// </summary>
    public bool PreferExplicitRequestBodyValidation { get; set; }

    /// <summary>
    /// The category name for the messages produced by the logger.
    /// <para>The default value is 'EndpointValidator'</para>
    /// </summary>
    public string LoggerCategoryName { get; set; } = "EndpointValidator";
}
