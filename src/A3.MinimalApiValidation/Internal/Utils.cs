namespace A3.MinimalApiValidation.Internal;

using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using A3.MinimalApiValidation.Internal.Middleware;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

internal static class Utils
{
    public static ILogger GetLogger<T>(this HttpContext context)
    {
        var logger = context.RequestServices.GetService<ILoggerFactory>()
            ?.CreateLogger($"A3.MinimalApiValidation.{typeof(T).Name}") ?? NullLogger.Instance;

        return logger;
    }

    public static JsonSerializerOptions GetJsonOptions(this HttpContext context)
    {
        var jsonOptions = context.RequestServices.GetService<EndpointValidatorOptions>()
                ?.JsonSerializerOptions
            ?? context.RequestServices.GetService<JsonOptions>()?.SerializerOptions
            ?? context.RequestServices.GetService<IOptions<JsonOptions>>()?.Value.SerializerOptions
            ?? new JsonSerializerOptions();

        return jsonOptions;
    }

    public static int Bit(this bool value) => value ? 1 : 0;

    public static bool IsMultiple(bool first, params bool[] others) => first.Bit() + others.Sum(Bit) > 1;

    public static string Serialize<T>(this T? value, JsonSerializerOptions? options)
    {
        return JsonSerializer.Serialize(value, options);
    }

    public static T? Deserialize<T>(this string json, JsonSerializerOptions? options)
    {
        return JsonSerializer.Deserialize<T>(json, options);
    }

    public static object? Deserialize(this string json, Type type, JsonSerializerOptions? options)
    {
        return JsonSerializer.Deserialize(json, type, options);
    }

    public static Task<ValidationResult> ValidateDataAnnotationsAsync(object? value, EndpointValidatorOptions options)
    {
        return Task.FromResult(ValidateDataAnnotations(value, options));
    }

    private static ValidationResult ValidateDataAnnotations(object? value, EndpointValidatorOptions options)
    {
        if (!options.FallbackToDataAnnotations)
        {
            return new ValidationResult();
        }

        if (value is null)
        {
            return new ValidationResult([new ValidationFailure("requestBody", "Value cannot be null.")]);
        }

        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(value);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(value, context, results, validateAllProperties: true);

        var errors = isValid
            ? []
            : results.Select(x => new ValidationFailure(x.MemberNames.FirstOrDefault() ?? "", x.ErrorMessage)).ToList();

        return new ValidationResult(errors);
    }

    public static bool TryDeserialize(
        string json,
        Type type,
        JsonSerializerOptions? options,
        out object? value,
        out List<ValidationFailure> errors)
    {
        value = null;
        errors = [];

        try
        {
            value = json.Deserialize(type, options);
            return true;
        }
        catch (Exception ex) when (ex is not ValidationException)
        {
            if (ex is JsonException jex)
            {
                errors.Add(new ValidationFailure(jex.Path, "The JSON value could not be converted."));
            }
            else if (ex.InnerException is JsonException jexInner)
            {
                errors.Add(new ValidationFailure(jexInner.Path, "The JSON value could not be converted."));
            }
            else
            {
                errors.Add(new ValidationFailure("requestBody", "Error binding request object."));
            }

            return false;
        }
    }

    /// <summary>
    /// Determines whether <paramref name="type"/> should be treated as a collection of values for
    /// header/query validation. <see cref="string"/> is intentionally excluded (it is an
    /// <see cref="IEnumerable{Char}"/> but must be validated as a scalar).
    /// </summary>
    public static bool IsCollectionType(Type type, [NotNullWhen(true)] out Type? elementType)
    {
        if (type == typeof(string))
        {
            elementType = null;
            return false;
        }

        return IsEnumerable(type, out elementType);
    }

    public static bool IsEnumerable(Type type, [NotNullWhen(true)] out Type? firstUnderlyingType)
    {
        if (!typeof(IEnumerable).IsAssignableFrom(type))
        {
            firstUnderlyingType = null;
            return false;
        }

        if (type is { IsInterface: true, IsGenericType: true }
            && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            firstUnderlyingType = type.GetGenericArguments()[0];
            return true;
        }

        firstUnderlyingType = type
            .GetInterfaces()
            .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            ?.GetGenericArguments()[0];

        return firstUnderlyingType is not null;
    }

    public static async Task<BodyValidationResult> ValidateCollectionAsync<T>(
        IEnumerable<T> collection,
        IValidator? validator,
        EndpointValidatorOptions options,
        Func<T, IValidationContext> getContext
    )
    {
        var results = new List<ValidationFailure>();
        var failedIndexes = new List<int>();
        foreach (var item in collection.Select((v, i) => new { Index = i, Value = v }))
        {
            var itemResult = await (validator is null
                ? ValidateDataAnnotationsAsync(item.Value, options)
                : validator.ValidateAsync(getContext(item.Value)));

            if (itemResult.IsValid)
            {
                continue;
            }

            var failures = itemResult.Errors.Select(e => new ValidationFailure($"item[{item.Index}].{e.PropertyName}", e.ErrorMessage));
            results.AddRange(failures);
            failedIndexes.Add(item.Index);
        }

        return new BodyValidationResult(
            results,
            $"{failedIndexes.Count} items in the array failed validation, indexes: {string.Join(", ", failedIndexes)}");
    }

    public static object? CastValueOrDefault(string? value, Type type)
    {
        var didCast = TryCastValue(value, type, out var castValue, out var defaultValue);
        return didCast ? castValue : defaultValue;
    }

    public static bool TryCastValue(string? value, Type type, out object? castValue, out object? defaultValue)
    {
        castValue = null;
        defaultValue = null;

        if (TypeParsers.TryGetValue(type, out var parser))
        {
            var (success, result, def) = parser(value);
            defaultValue = def;

            if (success)
            {
                castValue = result;
                return true;
            }

            castValue = defaultValue;
            return false;
        }

        // fall back to a parse convention (enums, IParsable<T>, or a static TryParse method).
        // this lets us validate any type the minimal api framework can bind, e.g. custom
        // "smart enum" types that expose a static TryParse(string, out T).
        var conventionParser = ConventionParsers.GetOrAdd(type, ResolveConventionParser);
        if (conventionParser is null)
        {
            return false;
        }

        var (ok, parsed) = conventionParser(value);
        if (ok)
        {
            castValue = parsed;
            return true;
        }

        return false;
    }

    private static ConcurrentDictionary<Type, Func<string?, (bool Success, object? Value)>?> ConventionParsers { get; } = new();

    private static Func<string?, (bool Success, object? Value)>? ResolveConventionParser(Type type)
    {
        // 1. real enums
        if (type.IsEnum)
        {
            return v =>
            {
                if (v is null)
                {
                    return (false, null);
                }

                var ok = Enum.TryParse(type, v, ignoreCase: false, out var result);
                return (ok, ok ? result : null);
            };
        }

        // 2. IParsable<T>: static bool TryParse(string?, IFormatProvider?, out T)
        var parsable = type.GetMethod(
            "TryParse",
            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy,
            binder: null,
            types: [typeof(string), typeof(IFormatProvider), type.MakeByRefType()],
            modifiers: null);

        if (parsable is not null && parsable.ReturnType == typeof(bool))
        {
            return v =>
            {
                var args = new object?[] { v, null, null };
                var ok = (bool) parsable.Invoke(null, args)!;
                return (ok, ok ? args[2] : null);
            };
        }

        // 3. legacy convention: static bool TryParse(string, out T)
        //    walk the hierarchy so statics declared on a generic base (e.g. SmartEnum<T>) are found.
        var legacy = FindLegacyTryParse(type);
        if (legacy is not null)
        {
            return v =>
            {
                var args = new object?[] { v, null };
                var ok = (bool) legacy.Invoke(null, args)!;
                return (ok, ok ? args[1] : null);
            };
        }

        return null;
    }

    private static MethodInfo? FindLegacyTryParse(Type type)
    {
        for (var current = type; current is not null && current != typeof(object); current = current.BaseType)
        {
            var method = current
                .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .FirstOrDefault(m =>
                    m is { Name: "TryParse", ReturnType.Name: nameof(Boolean) }
                    && m.GetParameters() is [{ } p0, { IsOut: true } p1]
                    && p0.ParameterType == typeof(string)
                    && p1.ParameterType.GetElementType() == type);

            if (method is not null)
            {
                return method;
            }
        }

        return null;
    }

    private static Dictionary<Type, Func<string?, (bool Success, object? CastValue, object? DefaultValue)>> TypeParsers { get; } = new()
    {
        { typeof(bool), v => (bool.TryParse(v, out var result), result, false) },
        { typeof(bool?), v => (bool.TryParse(v, out var result), result, null) },
        { typeof(int), v => (int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result), result, default(int)) },
        { typeof(int?), v => (int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result), result, null) },
        { typeof(long), v => (long.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result), result, default(long)) },
        { typeof(long?), v => (long.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result), result, null) },
        { typeof(float), v => (float.TryParse(v, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result), result, default(float)) },
        { typeof(float?), v => (float.TryParse(v, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result), result, null) },
        { typeof(double), v => (double.TryParse(v, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result), result, default(double)) },
        { typeof(double?), v => (double.TryParse(v, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result), result, null) },
        { typeof(decimal), v => (decimal.TryParse(v, NumberStyles.Number, CultureInfo.InvariantCulture, out var result), result, default(decimal)) },
        { typeof(decimal?), v => (decimal.TryParse(v, NumberStyles.Number, CultureInfo.InvariantCulture, out var result), result, null) },
        { typeof(DateTime), v => (DateTime.TryParse(v, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result), result, default(DateTime)) },
        { typeof(DateTime?), v => (DateTime.TryParse(v, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result), result, null) },
        { typeof(DateOnly), v => (DateOnly.TryParse(v, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result), result, default(DateOnly)) },
        { typeof(DateOnly?), v => (DateOnly.TryParse(v, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result), result, null) },
        { typeof(Guid), v => (Guid.TryParse(v, out var result), result, default(Guid)) },
        { typeof(Guid?), v => (Guid.TryParse(v, out var result), result, null) },
        { typeof(string), v => (true, v, default(string)) },
    };
}
