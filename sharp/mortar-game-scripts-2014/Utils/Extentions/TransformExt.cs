using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public static class TransformExt
{
    public static void SetX(this Transform transform, float x)
    {
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    public static void SetY(this Transform transform, float y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    public static void SetZ(this Transform transform, float z)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }

    public static void SetLocalX(this Transform transform, float x)
    {
        transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
    }

    public static void SetLocalY(this Transform transform, float y)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
    }

    public static void SetLocalZ(this Transform transform, float z)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
    }

    public static void AddX(this Transform transform, float x)
    {
        transform.position += new Vector3(x, 0, 0);
    }

    public static void AddY(this Transform transform, float y)
    {
        transform.position += new Vector3(0, y, 0);
    }

    public static void AddZ(this Transform transform, float z)
    {
        transform.position += new Vector3(0, 0, z);
    }

    public static void SetRotationEulerY(this Transform transform, float y)
    {
        transform.rotation =
            Quaternion.Euler(transform.rotation.eulerAngles.x, y, transform.rotation.eulerAngles.z);
    }
    public static void SetRotationEulerX(this Transform transform, float x)
    {
        transform.rotation =
            Quaternion.Euler(x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
    public static void SetRotationEulerZ(this Transform transform, float z)
    {
        transform.rotation =
            Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, z);
    }


    /// <summary>
    /// Сброс localPosition и localRotation в нуль и localScale в 1
    /// </summary>
    public static void LocalReset(this Transform tr)
    {
        tr.localPosition = Vector3.zero;
        tr.localRotation = Quaternion.identity;
        tr.localScale = Vector3.one;
    }

    /// <summary>
    /// Копирование в текущий transform значений Position, Rotation, Scale, Parent из другого transform-а
    /// </summary>
    /// <param name="doCloneScale">при true выполняет копирование Scale</param>
    /// <param name="doCloneParent">при true значение Parent устанавливается таким же как у копируемого transform-а</param>
    public static void CloneTransform(this Transform tr, Transform from, bool doCloneScale=false, bool doCloneParent=false)
    {
        tr.position = from.position;
        tr.rotation = from.rotation;
        if (doCloneScale)
            tr.localScale = from.localScale;
        if (doCloneParent)
            tr.parent = from.parent;
    }

    /// <summary>
    /// Поиск всех объектов с любым из переданных имен по всей дочерней иерархии текущего объекта
    /// </summary>
    /// <param name="excludeCurrentTransform">При true выполняет поиск только в дочерних объектах, не включая в результаты самого себя</param>
    public static Transform[] FindChildsRecursive(this Transform obj, bool excludeCurrentTransform = false, params string[] names)
    {
        if (excludeCurrentTransform)
            return obj.GetComponentsInChildren<Transform>().Where(tr => names.Contains(tr.name) && obj != tr).ToArray();
        else
            return obj.GetComponentsInChildren<Transform>().Where(tr => names.Contains(tr.name)).ToArray();
    }

    public static Transform[] GetChilds(this Transform transform, bool excludeInactive = false)
    {
        if (excludeInactive)
            return transform.Cast<Transform>().Where(tr => tr.gameObject.activeInHierarchy).ToArray();
        return transform.Cast<Transform>().ToArray();
    }

}