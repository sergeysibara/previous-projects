using System;
using System.Reflection;

/// <summary>
/// Помечает поле, которое нужно исключить.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ExcludeAttribute : Attribute
{
}

public static class AttributeUtils
{
    public static bool HasAttibute<T>(Object value) where T : Attribute
    {
        Type type = value.GetType();
        FieldInfo fi = type.GetField(value.ToString());
        T[] attrs = fi.GetCustomAttributes(typeof (T), false) as T[];
        return (attrs.Length > 0);
    }
}
