namespace A3.MinimalApiValidation.ValidationAttributes;

using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

internal static class ValidationContextExtensions
{
    /// <summary>
    /// A convenience method to create a new <see cref="ValidationResult"/> with an error message
    /// from the current <see cref="ValidationContext"/>.
    /// </summary>
    /// <param name="context">The current <see cref="ValidationContext"/></param>
    /// <param name="message">The validation error message to include.</param>
    /// <returns>
    /// A new <see cref="ValidationResult"/> with the given message, including the context display name and member name.
    /// </returns>
    public static ValidationResult Error(this ValidationContext context, string message)
    {
        return new ValidationResult($"{context.DisplayName} {message}", [context.MemberName ?? ""]);
    }

    /// <summary>
    /// Gets the value of another property from the current <see cref="ValidationContext"/>.
    /// <para>
    /// It will first try to get the value of a property on the same object as property
    /// being validated. If the property is not found, it will try to get the value from
    /// the query string or headers of the current <see cref="HttpContext"/>.
    /// </para>
    /// </summary>
    /// <param name="context">The current <see cref="ValidationContext"/></param>
    /// <param name="propertyName">The name of the property to resolve.</param>
    /// <returns>
    /// The value of the other property if it can be found from the current object,
    /// query string, or request headers; otherwise, <c>null</c>.
    /// </returns>
    public static object? GetOtherPropertyValue(this ValidationContext context, string propertyName)
    {
        // try get other property from current object
        var otherProperty = context
            .ObjectType
            .GetRuntimeProperty(propertyName);

        if (otherProperty is not null)
        {
            return otherProperty.GetValue(context.ObjectInstance, null);
        }

        // otherwise, try get the other property from query or header
        var httpContext = context.GetService<IHttpContextAccessor>()?.HttpContext;

        return
            httpContext?.Request.Query[propertyName].FirstOrDefault()
            ?? httpContext?.Request.Headers[propertyName].FirstOrDefault();
    }
}
