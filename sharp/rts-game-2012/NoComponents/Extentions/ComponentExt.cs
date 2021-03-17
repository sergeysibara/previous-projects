using UnityEngine;

public static class ComponentExt
{
    public static bool IsUnit(this Component obj)
    {
        return (obj.tag == Tags.Unit);
    }

    public static bool IsBuilding(this Component obj)
    {
        return (obj.tag == Tags.Building);
    }
}
