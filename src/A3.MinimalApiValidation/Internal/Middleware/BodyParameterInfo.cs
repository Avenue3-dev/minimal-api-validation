namespace A3.MinimalApiValidation.Internal.Middleware;

using FluentValidation;

internal record BodyParameterInfo
{
    public BodyParameterInfo(Type parameterType)
    {
        IsEnumerable = Utils.IsEnumerable(parameterType, out var underlyingType);
        
        if (IsEnumerable && underlyingType is null)
        {
            throw new InvalidOperationException("Failed to get underlying type for enumerable.");
        }

        ValidatorType = IsEnumerable && underlyingType is not null
            ? typeof(IValidator<>).MakeGenericType(underlyingType)
            : typeof(IValidator<>).MakeGenericType(parameterType);
        
        ValidationContextType = IsEnumerable && underlyingType is not null
            ? typeof(ValidationContext<>).MakeGenericType(underlyingType)
            : typeof(ValidationContext<>).MakeGenericType(parameterType);
    }


    public Type ValidatorType { get; }

    public Type ValidationContextType { get; }

    public bool IsEnumerable { get; }
    
    public IValidationContext CreateValidationContext(object value)
    {
        return Activator.CreateInstance(ValidationContextType, value) as IValidationContext
            ?? throw new InvalidOperationException($"Failed to create validation context for {ValidationContextType.Name}.");
    }
}
