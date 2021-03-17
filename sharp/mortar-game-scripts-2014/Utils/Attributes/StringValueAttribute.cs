using System;
using System.Reflection;

/// <summary>
/// Базовый строковый атрибут. Используется для задания строкового значения поля.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class StringValueAttribute : Attribute
{
    protected readonly string _value;

    public StringValueAttribute(string value)
    {
        _value = value;
    }

    public string Value
    {
        get { return _value; }
    }
}

public static class EnumExt
{
    /// <summary>
    /// Получение строки из атрибута StringValue или его наследников
    /// </summary>
    public static string GetStringValue<T>(this Enum value) where T : StringValueAttribute
    {
        string retValue = null;
        Type type = value.GetType();

        FieldInfo fi = type.GetField(value.ToString());
        T[] attrs = fi.GetCustomAttributes(typeof(T), false) as T[];
        if (attrs.Length > 0)
            retValue = attrs[0].Value;

        return retValue;
    }
}
