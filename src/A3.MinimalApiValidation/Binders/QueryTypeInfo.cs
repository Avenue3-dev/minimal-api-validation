namespace A3.MinimalApiValidation.Binders;

using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

internal class QueryTypeInfo
{
    private static ConcurrentDictionary<Type, QueryTypeInfo> QueryTypeInfoCache { get; } = new();

    public static QueryTypeInfo Get<T>()
    {
        var type = typeof(T);
        if (QueryTypeInfoCache.TryGetValue(type, out var info))
        {
            return info;
        }

        info = new QueryTypeInfo(type);
        QueryTypeInfoCache.TryAdd(type, info);
        return info;
    }

    private QueryTypeInfo(Type type)
    {
        var constructors =
            type.GetConstructors()
                .Select(x => x.GetParameters())
                .OrderByDescending(x => x.Length)
                .ToArray();

        if (constructors.Length == 0)
        {
            throw new InvalidOperationException($"No public constructors found for type {type.Name}");
        }

        var properties = type.GetProperties()
            .Select(x => (
                Info: x,
                QueryName: x.GetCustomAttribute<FromQueryAttribute>()?.Name
            ))
            .ToArray();

        HasParameterlessConstructor = constructors[^1].Length == 0;

        Parameters = constructors[0]
            .Select(param => (
                Type: param.ParameterType,
                HasDefaultValue: param.HasDefaultValue,
                Name: param.Name ?? throw new InvalidOperationException("Parameter name cannot be null."),
                QueryName: properties.FirstOrDefault(x => x.Info.Name.Equals(param.Name, StringComparison.OrdinalIgnoreCase)).QueryName,
                DefaultValue: param.DefaultValue
            ))
            .ToArray();

        Type = type;
        Properties = properties;
    }

    public Type Type { get; }

    private bool HasParameterlessConstructor { get; }

    private (PropertyInfo Info, string? QueryName)[] Properties { get; }

    private (Type Type, bool HasDefaultValue, string Name, string? QueryName, object? DefaultValue)[] Parameters { get; }

    public object? CreateInstance(IQueryCollection query)
    {
        if (HasParameterlessConstructor)
        {
            var instance = Activator.CreateInstance(Type);

            foreach (var property in Properties)
            {
                var name = property.QueryName ?? property.Info.Name;

                if (query.TryGetValue(name, out var value))
                {
                    var convertedValue = GetValue(property.Info.PropertyType, value);
                    property.Info.SetValue(instance, convertedValue);
                }
            }

            return instance;
        }

        var args = new object?[Parameters.Length];

        for (var i = 0; i < Parameters.Length; i++)
        {
            var parameter = Parameters[i];
            var name = parameter.QueryName ?? parameter.Name;

            if (name is null)
            {
                throw new InvalidOperationException("Parameter name is required but was null.");
            }

            if (query.TryGetValue(name, out var value))
            {
                args[i] = GetValue(parameter.Type, value);
            }
            else if (parameter.HasDefaultValue)
            {
                args[i] = parameter.DefaultValue;
            }
            else
            {
                args[i] = parameter.Type.IsValueType
                    ? Activator.CreateInstance(parameter.Type)
                    : null;
            }
        }

        return Activator.CreateInstance(Type, args);
    }

    private static object? GetValue(Type type, StringValues value)
    {
        if (type.IsArray)
        {
            var elementType = type.GetElementType();
            if (elementType == null)
            {
                return null;
            }

            var array = Array.CreateInstance(elementType, value.Count);
            for (var i = 0; i < value.Count; i++)
            {
                array.SetValue(CastValue(value[i], elementType), i);
            }

            return array;
        }

        if (typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
        {
            var elementType = type.GetGenericArguments()[0];
            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = (IList) Activator.CreateInstance(listType)!;

            foreach (var item in value)
            {
                list.Add(CastValue(item, elementType));
            }

            return list;
        }

        return CastValue(value.ToString(), type);
    }

    private static object? CastValue(string? value, Type type)
    {
        return type switch
        {
            _ when type == typeof(bool) => bool.TryParse(value, out var b) && b,
            _ when type == typeof(bool?) => bool.TryParse(value, out var b) ? b : null,
            _ when type == typeof(int) => int.TryParse(value, out var n) ? n : 0,
            _ when type == typeof(int?) => int.TryParse(value, out var n) ? n : null,
            _ when type == typeof(long) => long.TryParse(value, out var n) ? n : 0,
            _ when type == typeof(long?) => long.TryParse(value, out var n) ? n : null,
            _ when type == typeof(float) => float.TryParse(value, out var n) ? n : 0,
            _ when type == typeof(float?) => float.TryParse(value, out var n) ? n : null,
            _ when type == typeof(double) => double.TryParse(value, out var n) ? n : 0,
            _ when type == typeof(double?) => double.TryParse(value, out var n) ? n : null,
            _ when type == typeof(decimal) => decimal.TryParse(value, out var n) ? n : 0,
            _ when type == typeof(decimal?) => decimal.TryParse(value, out var n) ? n : null,
            _ when type == typeof(DateTime) => DateTime.TryParse(value, out var dt) ? dt : DateTime.MinValue,
            _ when type == typeof(DateTime?) => DateTime.TryParse(value, out var dt) ? dt : null,
            _ when type == typeof(Guid) => Guid.TryParse(value, out var dt) ? dt : Guid.Empty,
            _ when type == typeof(Guid?) => Guid.TryParse(value, out var dt) ? dt : null,
            _ when type == typeof(string) => value,
            _ => default,
        };
    }
}
