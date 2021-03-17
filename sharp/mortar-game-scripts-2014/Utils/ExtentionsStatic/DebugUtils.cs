using UnityEngine;
using System.Collections;

public static class DebugUtils
{
    public static void DrawVerticalRay(Vector3 position, float size, Color color)
    {
        Debug.DrawRay(position, Vector3.up*size, color,100f);
    }

    public static string AddColor(this string text, DColor c)
    {
        return string.Format("<color={0}>{1}</color>", c, text);
    }

    public enum DColor
    {
        red,
        blue,
        brown,
        cyan,
        magenta,
        green,
        grey,
        lime,
        lightblue,
        olive,
        orange,
        purple,
        yellow,
        white,
        darkblue,
    }
}