using System.Collections.Generic;
using System.Linq;

namespace UnityEngine
{
    public static class RandomUtils
    {
        public static int RangeWithoutExcludeds(int min, int max, params int[] excludedValues)
        {
            if (min >= (max - 1))
            {
                Debug.LogError("не корректный диапазон");
                return min;
            }

            int rnd;
            do
            {
                rnd = Random.Range(min, max);
            } while (excludedValues.Any(v=>v==rnd));
            return rnd;
        }

        public static T GetRandomItem<T>(IEnumerable<T> values)
        {
            var enumerable = values as T[] ?? values.ToArray();
            if (enumerable.Count() == 0)
                return default(T);
            var index = Random.Range(0, enumerable.Count());
            return enumerable.ElementAt(index);
        }

        public static T GetRandomWithoutExcludeds<T>(IEnumerable<T> values, params T[] excludedValues)
        {
            IEnumerable<T> enumerableWithoutExcluded = values.Except(excludedValues);

            var enumerable = enumerableWithoutExcluded as T[] ?? enumerableWithoutExcluded.ToArray();
            if (enumerable.Count() == 0)
                return default(T);
            var index = Random.Range(0, enumerable.Count());
            return enumerable.ElementAt(index);
        }

        public static Vector3 PointInsideCircle(Vector3 center, float radius, LayerMask groundLayerMask)
        {
            Vector2 new2dPos = Random.insideUnitCircle * radius;
            Vector3 new3dPos = center + new Vector3(new2dPos.x, 0, new2dPos.y);
            return PhysicsUtils.RaycastFromUpToDown(new3dPos, groundLayerMask).point;
        }
    }
}
