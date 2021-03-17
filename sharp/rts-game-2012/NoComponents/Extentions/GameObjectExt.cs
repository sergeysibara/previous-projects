using UnityEngine;
using System.Collections.Generic;

public static class GameObjectExt
{
    public static bool IsDied(this GameObject obj)
    {
        return (obj.layer == GameManager.DefaultLayer);
    }

    public static bool IsUnit(this GameObject obj)
    {
        return (obj.tag == Tags.Unit);
    }

    public static bool IsBuilding(this GameObject obj)
    {
        return (obj.tag == Tags.Building);
    }

}