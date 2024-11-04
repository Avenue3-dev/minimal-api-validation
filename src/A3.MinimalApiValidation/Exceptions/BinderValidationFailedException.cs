namespace A3.MinimalApiValidation.Exceptions;

using FluentValidation.Results;

[Serializable]
public class BinderValidationFailedException : Exception
{
    public BinderValidationFailedException(string propertyName, string error)
    {
        Errors = new[] { new ValidationFailure(propertyName, error) };
    }
    internal BinderValidationFailedException(IEnumerable<ValidationFailure> errors)
    {
        Errors = errors;
    }
    
    internal BinderValidationFailedException(IEnumerable<ValidationFailure> errors, string detail)
    {
        Errors = errors;
        Detail = detail;
    }

    public IEnumerable<ValidationFailure> Errors { get; }

    public string? Detail { get; }
}
