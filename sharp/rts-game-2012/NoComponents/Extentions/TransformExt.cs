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

    public static I GetInterfaceComponent<I>(this Transform obj) where I : class
    {
        return obj.GetComponent(typeof(I)) as I;
    }

    public static I[] GetInterfaceComponents<I>(this Transform obj) where I : class
    {
        var components=obj.GetComponents(typeof(I));

        I[] Icomponents = new I[components.Length];
        components.CopyTo(Icomponents, 0);

        return Icomponents;
    }

    public static T GetSafeComponent<T>(this Transform obj) where T : Component
    {
        T component = obj.GetComponent<T>();

        if (component == null)
            Debug.LogError("Component of type " + typeof(T) + " not found", obj);

        return component;
    }

    public static Transform FindSafeChild(this Transform obj, string name)
    {
        Transform tr = obj.FindChild(name);

        if (tr == null)
            Debug.LogError("Child game object with " + name + " not found", obj);

        return tr;
    }

}