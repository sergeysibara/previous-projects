using System;
using UnityEngine;
using System.Collections.Generic;

namespace Shapes
{
    ///<remarks>Значения должны совпадать со значениями в классе Shape</remarks>
    [System.Serializable]
    public enum Direction
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3,
        None = 4
    }

    public static class DirectionExt
    {
        public static Direction GetNext(this Direction dir)
        {
            if ((sbyte) dir + 1 > 3)
                return 0;
            return dir + 1;
        }

        public static Direction GetPrev(this Direction dir)
        {
            if ((sbyte) dir - 1 < 0)
                return (Direction) 3;
            return dir - 1;
        }

        /// <summary>
        /// Противоположное значение: для Up возвращает down, для left вернет Right 
        /// </summary>
        public static Direction GetOpposite(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    return Direction.Down;
                case Direction.Right:
                    return Direction.Left;
                case Direction.Down:
                    return Direction.Up;
                default:
                    return Direction.Right;
            }
        }

        /// <summary>
        /// Для Left и Down возвращает -1, для остальных значений возвращает 1.
        /// </summary>
        public static int CreateSign(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Left:
                case Direction.Down:
                    return -1;
                default:
                    return 1;
            }
        }
    }


    public static class DirectionUtils
    {
        public static Direction EulerAngleToDirection(float angle)
        {
            switch (Mathf.RoundToInt(angle))
            {
                case 0:
                    return Direction.Up;
                case 90:
                    return Direction.Right;
                case 180:
                    return Direction.Down;
                case 270:
                    return Direction.Left;
                default:
                    Debug.LogError("недопустимое значение угла");
                    return Direction.None;
            }
        }

        public static Vector3 DirectionToVector3(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Vector3.forward;
                case Direction.Right:
                    return Vector3.right;
                case Direction.Down:
                    return Vector3.back;
                case Direction.Left:
                    return Vector3.left;
                default:
                    Debug.LogError("недопустимое значение угла");
                    return Vector3.zero;
            }
        }

        public static Quaternion DirectionToQuaternion(Direction direction)
        {
            return Quaternion.LookRotation(DirectionToVector3(direction));
        }
    }
}