namespace A3.MinimalApiValidation;

using A3.MinimalApiValidation.Internal.Filter;
using Microsoft.Extensions.Options;

public class EndpointValidatorOptions
{
    /// <summary>
    /// Gets the default options for <see cref="EndpointValidatorOptions"/>.
    /// </summary>
    public static EndpointValidatorOptions Default { get; } = new();
    
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
    public System.Text.Json.JsonSerializerOptions? JsonSerializerOptions { get; set; }

    /// <summary>
    /// If set to <c>true</c>, request model validation (body or query groups) will only be performed if
    /// the endpoint uses the <see cref="RequestModelValidationFilter{T}" /> endpoint filter.
    /// <para>The default value is <c>false</c></para>
    /// </summary>
    public bool PreferExplicitRequestModelValidation { get; set; }
    
    /// <summary>
    /// Used to set the name for the validate only header for use with the <see cref="ValidateOnlyFilter"/>.
    /// <para>The default value is 'x-validate-only'.</para>
    /// </summary>
    public string ValidateOnlyHeader { get; set; } = "x-validate-only";
}
