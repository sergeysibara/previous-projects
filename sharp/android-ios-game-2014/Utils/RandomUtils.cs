using System.Collections.Generic;
using System.Linq;

namespace UnityEngine
{
    public static class RandomUtils
    {
        public static int RangeWithExclude(int min, int max, params int[] values)
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
            } while (values.Any(v=>v==rnd));
            return rnd;
        }

        public static T GetRandomItem<T>(IEnumerable<T> values)
        {
            var enumerable = values as T[] ?? values.ToArray();
            var index = Random.Range(0, enumerable.Count());
            return enumerable.ElementAt(index);
        }
    }
}
