namespace UnityEngine
{
    public static class ColorUtils
    {
        public static Color CreateWithAlpha(Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }
    }
}
