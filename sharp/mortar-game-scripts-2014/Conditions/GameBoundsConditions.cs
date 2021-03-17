using UnityEngine;
using System.Collections;

namespace Conditions
{
    public static class GameBounds
    {
        private const float _nearBoundsOffset = 15;
        private const float _farBoundsOffset = 5;

        public static bool IsInsidePlayerShootBounds(Vector3 posintion)
        {
            Vector2 pos2d = new Vector2(posintion.x, posintion.z);
            var rect = RTSCamera.Instance.GetBounds();
            rect.yMin += _nearBoundsOffset;//корректировка позиция с учетом области стрельбы игрока

            //DebugUtils.DrawRect(rect,30,Color.blue);
            //DebugUtils.DrawVerticalRay(posintion,20,Color.blue);
            return rect.Contains(pos2d);
        }

        //Если позиция ближе к игроку, чем граница допустимрй области стрельбы игрока
        public static bool IsCloserThanPlayerShootBounds(Vector3 posintion)
        {
            var rect = RTSCamera.Instance.GetBounds();
            if (posintion.z <= rect.yMin + _nearBoundsOffset)
                return true;
            return false;
        }

        public static bool IsInsideCameraMovementBounds(Vector3 posintion)
        {
            Vector2 pos2d = new Vector2(posintion.x, posintion.z);
            var rect = RTSCamera.Instance.GetBounds();
            rect.yMax += _farBoundsOffset;
            //DebugUtils.DrawRect(rect,30,Color.blue);
            //DebugUtils.DrawVerticalRay(posintion,20,Color.blue);
            return rect.Contains(pos2d);
        }
    }
}
