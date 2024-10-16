namespace EndpointValidator.Internal.Middleware;

using FluentValidation.Results;

internal record BodyValidationResult
{
    public List<ValidationFailure> Errors { get; }

    public string? Detail { get; }

    public BodyValidationResult(List<ValidationFailure> errors, string? detail)
    {
        Errors = errors;
        Detail = detail;
    }

    public static implicit operator BodyValidationResult(List<ValidationFailure> errors)
    {
        return new BodyValidationResult(errors, null);
    }

    public static implicit operator BodyValidationResult(ValidationFailure error)
    {
        return new BodyValidationResult([error], null);
    }
}