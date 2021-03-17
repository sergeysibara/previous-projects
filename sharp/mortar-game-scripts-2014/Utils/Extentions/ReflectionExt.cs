using System;
using System.Linq.Expressions;
using System.Reflection;

public static class TypeExt
{
    public static bool HasAttribute<T>(this Type type, bool inherit = false) where T : Attribute
    {
        return type.GetCustomAttributes(typeof (T), inherit).Length > 0;
    }
}

public static class ReflectionExt
{
    public static MethodInfo GetMethod<T>(this T instance,
                                          Expression<Func<T, object>> methodSelector)
    {
        // Note: this is a bit simplistic implementation. It will
        // not work for all expressions.
        return ((MethodCallExpression) methodSelector.Body).Method;
    }

    public static MethodInfo GetMethod<T>(this T instance,
                                          Expression<Action<T>> methodSelector)
    {
        return ((MethodCallExpression) methodSelector.Body).Method;
    }

    public static bool HasAttribute<TAttribute>(
        this MemberInfo member)
        where TAttribute : Attribute
    {
        return GetAttributes<TAttribute>(member).Length > 0;
    }

    public static TAttribute[] GetAttributes<TAttribute>(
        this MemberInfo member)
        where TAttribute : Attribute
    {
        var attributes =
            member.GetCustomAttributes(typeof (TAttribute), true);

        return (TAttribute[]) attributes;
    }
}
