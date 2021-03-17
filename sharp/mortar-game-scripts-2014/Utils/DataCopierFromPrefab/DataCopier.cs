using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Копирует отображаемые в интспекторе данные из одного MonoBehaviour скрипта в другой скрипт с таким же типом.
/// </summary>
public static class DataCopier 
{
    public static void GetAndFill<T>(T fromObj, T toObj, params string[] exludedFiels) where T : MonoBehaviour
    {
        Type type = typeof(T);
        HashSet<FieldInfo> desiredFields = new HashSet<FieldInfo>();

        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default);
        desiredFields.UnionWith(fields);

        //получаем приватные поля из классов-предков
        if (type != typeof(MonoBehaviour))
        {
            var parentType = type;
            for (int i = 0; i < 10; i++)
            {
                parentType = parentType.BaseType;
                if (parentType == typeof(MonoBehaviour))
                    break;
                fields = parentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default);
                desiredFields.UnionWith(fields);
            }
        }

        foreach (var fieldInfo in desiredFields)
        {
            if (Array.IndexOf(exludedFiels, fieldInfo.Name) > -1)
                continue;

            if (fieldInfo.Name[0] == '<') //исключает поля, объявленные таким образом:   public int E { get; private set; } 
                continue;

            if (fieldInfo.HasAttribute<HideInInspector>())
                continue;

            if (fieldInfo.IsPrivate && !fieldInfo.HasAttribute<SerializeField>())
                continue;

            var targetFieldValue = fieldInfo.GetValue(fromObj);//берем значение из скрипта в префабе
            fieldInfo.SetValue(toObj, targetFieldValue); //полю из целевого скрипта присваиваем значение.
        }

    }
}
