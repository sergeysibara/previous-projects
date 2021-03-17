using UnityEngine;
using System.Linq;
namespace Getters
{
    public static class TargetFinders
    {
        public static Transform FindNearestTarget(Vector3 seekerPos, float searchRadius, LayerMask targetLayerMask)
        {
            Collider[] targets = Physics.OverlapSphere(seekerPos, searchRadius, targetLayerMask);

            if (targets.Length != 0)
            {
                Transform nearTarget = GetNearest(seekerPos, targets).transform;
                return nearTarget;
            }

            return null;
        }

        public static T FindNearestTarget<T>(Vector3 seekerPos, float searchRadius, LayerMask targetLayerMask) where T : Component
        {
            T[] targets = PhysicsUtils.OverlapSphere<T>(seekerPos, searchRadius, targetLayerMask);

            if (targets.Length != 0)
            {
                T nearTarget = GetNearest(seekerPos, targets);
                return nearTarget;
            }

            return null;
        }

        public static Transform GetNearest(Vector3 seekerPos, Transform[] targets)
        {
            Transform nearTarget = null;
            float minDistSqr = float.MaxValue;

            foreach (var tr in targets)
            {
                float distSqr = (tr.position - seekerPos).sqrMagnitude;
                if (distSqr < minDistSqr)
                {
                    nearTarget = tr;
                    minDistSqr = distSqr;
                }
            }
            return nearTarget;
        }

        public static T GetNearest<T>(Vector3 seekerPos, T[] targets) where T : Component
        {
            if (typeof(T) == typeof(Transform))
            {
                Debug.LogError("The 'Transform' type argument is not supported");
                return null;
            }
            T nearTarget = null;
            float minDistSqr = float.MaxValue;

            foreach (var item in targets)
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
    }
}