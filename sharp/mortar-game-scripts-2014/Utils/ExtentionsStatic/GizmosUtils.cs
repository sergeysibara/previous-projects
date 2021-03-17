
using UnityEngine;
using System.Collections;


public static class GizmosUtils
{
    public static void DrawText(GUISkin guiSkin, string text, Vector3 position, Color? color = null, int fontSize = 0)
    {
#if UNITY_EDITOR
        var prevSkin = GUI.skin;
        if (guiSkin == null)
            Debug.LogWarning("editor warning: guiSkin==null");

        GUI.skin = guiSkin;
        GUIContent nameContent = new GUIContent(text);

        GUIStyle style = (guiSkin != null) ? new GUIStyle(guiSkin.GetStyle("Label")) : new GUIStyle();
        if (color != null)
            style.normal.textColor = (Color) color;
        if (fontSize > 0)
            style.fontSize = fontSize;


        Vector2 size = style.CalcSize(nameContent);
        Vector3 screenPoint = Camera.current.WorldToScreenPoint(position);
        position = Camera.current.ScreenToWorldPoint(new Vector3(screenPoint.x - size.x*0.5f, screenPoint.y, -screenPoint.z));
        UnityEditor.Handles.Label(position, nameContent, style);
        GUI.skin = prevSkin;
        #endif
    }

    /// <param name="countCells">Число ячеек, которые отобразяться</param>
    public static void DrawSquareGrid(Vector3 center, float size, float countCells)
    {#if UNITY_EDITOR
        Vector3 gridCenter = center + new Vector3(-size / 2f+0.5f, 0f, -size / 2f+0.5f);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (countCells<=i*size+j)
                    return;
                Gizmos.DrawCube(gridCenter+new Vector3(i, 0, j),new Vector3(0.9f,1f,0.9f));
            }
        }
        #endif
    }

    public static void DrawRect(Rect rect, float height, float startPosY)
    {
        #if UNITY_EDITOR
        var p1 = new Vector2(rect.xMin, rect.yMin);
        var p2 = new Vector2(rect.xMin, rect.yMax);
        var p3 = new Vector2(rect.xMax, rect.yMax);
        //var p4 = new Vector2(rect.xMax, rect.yMin);

        float zHeight = p2.y - p1.y;
        float xWidth = p3.x - p2.x;
        Gizmos.DrawCube(new Vector3(p1.x, startPosY, p1.y + zHeight * 0.5f), new Vector3(0.1f, height, zHeight)); //left
        Gizmos.DrawCube(new Vector3(p1.x + xWidth * 0.5f, startPosY, p2.y), new Vector3(xWidth, height, 0.1f));//top

        Gizmos.DrawCube(new Vector3(p3.x, startPosY, p1.y + zHeight * 0.5f), new Vector3(0.1f, height, zHeight)); //right
        Gizmos.DrawCube(new Vector3(p1.x + xWidth * 0.5f, startPosY, p1.y), new Vector3(xWidth, height, 0.1f));//bottom
        #endif
    }
}
