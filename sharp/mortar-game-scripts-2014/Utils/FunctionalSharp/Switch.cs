using System;

public static class Switch
{
    public static Switch<T> On<T>(T value)
    {
        return new Switch<T>(value);
    }
}

public class Switch<T>
{
    private bool hasBeenHandled;
    private readonly T value;

    public Switch(T value)
    {
        this.value = value;
    }

    public Switch<T> Case(T comparisonValue, Action action)
    {
        if (AreEqual(value, comparisonValue))
        {
            hasBeenHandled = true;
            action();
        }
        return this;
    }

    public Switch<T> Case(T comparisonValue,T comparisonValue2, Action action)
    {
        if (AreEqual(value, comparisonValue) || AreEqual(value, comparisonValue2))
        {
            hasBeenHandled = true;
            action();
        }
        return this;
    }


    public void Default(Action action)
    {
        if (!hasBeenHandled)
            action();
    }

    private bool AreEqual(T actualValue, T comparisonValue)
    {
        return Equals(actualValue, comparisonValue);
    }
}