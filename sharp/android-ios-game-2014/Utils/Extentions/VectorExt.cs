using UnityEngine;
using System.Collections;


    public static class Vector3Ext
    {
        public static Vector2 xz(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.z);
        }
    }

    public static class Vector2Ext
    {
        /// <param name="y">Значение y из Vector3</param>
        public static Vector3 ToVector3(this Vector2 vector2, float y)
        {
            return new Vector3(vector2.x, y, vector2.y);
        }
    }