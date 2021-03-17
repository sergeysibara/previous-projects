using System;

namespace UnityEngine
{
    public static class MathfUtils
    {
        public static float Frac(float value)
        {
            var ret = value - (float)Math.Truncate(value);
            if (ret > 0.9999f)
                ret = 0;
            return ret;
        }

        /// <summary>
        /// Переносит дробную часть в целую. Т.е. для (0.2) вернет 0.2*10=2
        /// </summary>
        public static int FracDecimalToCeil(float value)
        {
            //pow(10,n)
            return (int)Mathf.RoundToInt(Frac(value) * 10f);
        }
    }
}
