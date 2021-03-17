using System.Collections.Generic;
using UnityEngine;

public static class ComponentExt
{
    public static I GetInterfaceComponent<I>(this Component comp) where I : class
    {
        return comp.GetComponent(typeof(I)) as I;
    }

    public static I[] GetInterfaceComponents<I>(this Component comp) where I : class
    {
        var components = comp.GetComponents(typeof(I));
        I[] Icomponents = new I[components.Length];
        components.CopyTo(Icomponents, 0);
        return Icomponents;
    }

    public static T GetSafeComponent<T>(this Component comp) where T : Component
    {
        T component = comp.GetComponent<T>();
        if (component == null)
            Debug.LogError("Component of type " + typeof(T) + " not found", comp);
        return component;
    }

    public static T[] GetComponentsInDirectChildrens<T>(this Component self) where T : Component
    {
        var transform = self.transform;
        List<T> components = new List<T>();
        foreach (Transform tr in transform)
        {
            components.AddRange(tr.GetComponents<T>());
        }
        return components.ToArray();
    }
}