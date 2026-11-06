namespace A3.MinimalApiValidation.Tests._utils;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

// Simplified stand-ins for the consumer's string-backed "smart enum" patterns. Both expose the
// static TryParse(string, out T) convention on the generic base (inherited by the concrete type),
// which is what the minimal api framework uses to bind and what A3 uses to validate.

/// <summary>
/// A closed string-backed enumeration: only pre-defined values are valid, unknown values fail to parse.
/// </summary>
public abstract class ClosedEnumeration<T> where T : ClosedEnumeration<T>
{
    private static IEnumerable<T> AllItems { get; } = typeof(T)
        .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
        .Select(p => p.GetValue(null))
        .Cast<T>()
        .ToArray();

    public static bool TryParse(string? value, [NotNullWhen(true)] out T? result)
    {
        result = AllItems.FirstOrDefault(x => x.Value == value);
        return result is not null;
    }

    protected ClosedEnumeration(string value) => Value = value;

    public string Value { get; }

    public override string ToString() => Value;
}

public sealed class Animal : ClosedEnumeration<Animal>
{
    public static Animal Cat { get; } = new("cat");
    public static Animal Dog { get; } = new("dog");
    public static Animal Bird { get; } = new("bird");

    private Animal(string value) : base(value)
    {
    }
}

/// <summary>
/// An open string-backed enumeration: pre-defined values are recognised, but any other non-empty
/// value is accepted as free text (never fails to parse).
/// </summary>
public abstract class OpenEnumeration<T> where T : OpenEnumeration<T>
{
    private static IEnumerable<T> AllDefinedItems { get; } = typeof(T)
        .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
        .Select(p => p.GetValue(null))
        .Cast<T>()
        .ToArray();

    public static bool TryParse(string? value, [NotNullWhen(true)] out T? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        result = AllDefinedItems.FirstOrDefault(x => string.Equals(x.Value, value, StringComparison.OrdinalIgnoreCase))
            ?? (T) Activator.CreateInstance(
                typeof(T),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                binder: null,
                args: [value],
                culture: null)!;

        return true;
    }

    protected OpenEnumeration(string value) => Value = value;

    public string Value { get; }

    public override string ToString() => Value;
}

public sealed class Status : OpenEnumeration<Status>
{
    public static Status Active { get; } = new("active");
    public static Status Archived { get; } = new("archived");

    private Status(string value) : base(value)
    {
    }
}
