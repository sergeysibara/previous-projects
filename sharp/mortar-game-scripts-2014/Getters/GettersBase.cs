using UnityEngine;

namespace Getters
{
    public static class Base
    {
        public static float GetDistance(Vector3 position1, Vector3 position2)
        {
            return (position1 - position2).magnitude;
        }

        public static Vector3 GetDirection(Vector3 from, Vector3 to)
        {
            return (to - from);
        }

        public static Vector3 GetDirectionByXZ(Vector3 from, Vector3 to)
        {
            var ret = (to - from);
            ret.y = 0;
            //from.y;//0
            return ret;
        }

        public static float GetAngle(Transform transform, Vector3 targetDirection)
        {
            return Vector3.Angle(transform.forward, targetDirection);
        }

        public static float GetAngle(Transform transform, Transform target)
        {
            return Vector3.Angle(transform.forward, GetDirection(transform.position, target.position));
        }

        public static float GetAngleByXZ(Transform transform, Transform target)
        {
            return Vector3.Angle(transform.forward, GetDirectionByXZ(transform.position, target.position));
        }
    }
}