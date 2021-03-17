using UnityEngine;

public static class TargetFindingMethods
{
    public static Transform FindNearestTarget(Vector3 seekerPos, float searchRadius, LayerMask targetLayerMask)
    {
        Collider[] targets = Physics.OverlapSphere(seekerPos, searchRadius, targetLayerMask);

        if (targets.Length != 0)
        {
            Transform nearTarget = null;
            float minDistSqr = float.MaxValue;

            foreach (Collider c in targets)
            {
                Transform target = c.transform;

                float distSqr = (target.position - seekerPos).sqrMagnitude;
                if (distSqr < minDistSqr)
                {
                    nearTarget = target;
                    minDistSqr = distSqr;
                }
            }
            return nearTarget;
        }

        return null;
    }

    public static T FindNearestTarget<T>(Vector3 seekerPos, float searchRadius, LayerMask targetLayerMask) where T:Component
    {
        T[] targets = PhysicsExt.OverlapSphere<T>(seekerPos, searchRadius, targetLayerMask);

        if (targets.Length != 0)
        {
            T nearTarget = null;
            float minDistSqr = float.MaxValue;

            foreach (T item in targets)
            {
                Transform target = item.transform;

                float distSqr = (target.position - seekerPos).sqrMagnitude;
                if (distSqr < minDistSqr)
                {
                    nearTarget = item;
                    minDistSqr = distSqr;
                }
            }
            return nearTarget;
        }

        return null;
    }
}