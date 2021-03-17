using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PhysicsUtils
{
    public const float RaycastHeigthFromUpToDown = 1000;

    public static T[] OverlapSphere<T>(Vector3 position, float radius) where T : Component
    {
        var colladers = Physics.OverlapSphere(position, radius);
        return ExtractComponents<T>(colladers);
    }

    public static T[] OverlapSphere<T>(Vector3 position, float radius, int layerMask) where T : Component
    {
        var colladers = Physics.OverlapSphere(position, radius, layerMask);
        return ExtractComponents<T>(colladers);
    }

    /// <remarks>Проверка, был ли hit - отсутсвует</remarks>
    public static RaycastHit RaycastFromUpToDown(Vector3 position, LayerMask layers)
    {
        RaycastHit hit;
        position.y = RaycastHeigthFromUpToDown;
        Ray ray = new Ray(position, -Vector3.up);
        Physics.Raycast(ray, out hit, Mathf.Infinity, layers);
        return hit;
    }


    private static T[] ExtractComponents<T>(Collider[] colliders) where T : Component
    {
        List<T> retList = new List<T>();
        foreach (var c in colliders)
        {
            var targetComponent = c.GetComponent<T>();
            if (targetComponent != null)
                retList.Add(targetComponent);
        }
        return retList.ToArray();
    }

}