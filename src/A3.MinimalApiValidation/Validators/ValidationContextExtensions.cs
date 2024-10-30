namespace A3.MinimalApiValidation.Validators;

using System.ComponentModel.DataAnnotations;

internal static class ValidationContextExtensions
{
    public static ValidationResult Error(this ValidationContext context, string message)
    {
        return new ValidationResult($"{context.DisplayName} {message}", [context.MemberName ?? ""]);
    }
}
