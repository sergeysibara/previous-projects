using UnityEngine;

public static class PlacementUtils 
{
    /// <summary>
    /// Проверяет, есть ли объекты в данной позиции. 
    /// </summary>
    /// <param name="position">Центр для проверки через OverlapSphere. Желательно передавать высоту над землей, равную радиусу</param>
    public static bool IsFreePosition(Vector3 position, float radius, LayerMask mask, GameObject excludedObject=null)
    {
        Collider[] colls = Physics.OverlapSphere(position, radius, mask);
        if (colls.Length == 0)
            return true;
        if (colls.Length > 1 || excludedObject == null)
            return false;
        return colls[0].gameObject == excludedObject; //если объект в данной позиции является исключаемым
    }
}
