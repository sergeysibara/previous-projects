using System;
using System.Collections.Generic;

public enum CommandButtonId
{
    [CommandButtonType(typeof(AttackTarget_Button))]
    AttackTarget,

    [CommandButtonType(typeof(CreateWarrior_Button))]
    CreateWarrior,

    [CommandButtonType(typeof(CreateSkeleton_Button))]
    CreateSkeleton,
}

public class CommandButtonTypeAttribute : Attribute
{
    public CommandButtonTypeAttribute(Type value)
    {
        Value = value;
    }

    public Type Value { get; private set; }
}

public static class CommandButtonTypeGetter
{
    public static Type GetCommandButtonType(CommandButtonId id)
    {
        object[] attribs = typeof(CommandButtonId)
            .GetField(id.ToString())
            .GetCustomAttributes(typeof(CommandButtonTypeAttribute), false);

        if (attribs != null && attribs.Length > 0)
            return ((CommandButtonTypeAttribute)attribs[attribs.Length - 1]).Value;

        return null;
    }

    public static List<Type> GetCommandButtonTypes(CommandButtonInfo[] commandButtonInfoArray)
    {
        List<Type> buttonTypeList = new List<Type>();

        foreach (CommandButtonInfo buttonInfo in commandButtonInfoArray)
        {
            buttonTypeList.Add(GetCommandButtonType(buttonInfo.ButtonId));
        }
        return buttonTypeList;
    }
}