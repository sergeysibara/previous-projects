using UnityEngine;
using System.Collections;

namespace Conditions
{  
    public static class GUI
    {
        /// <summary>
        /// Возвращает true, если позиция мыши или Touch находится над gui элементом. На gui элементы должен быть коллайдер.
        /// </summary>
        public static bool ClickOverGUI(Camera cam, LayerMask uiMask)
        {
            Vector3 pos;
            RaycastHit hit;
            if (UnityEngine.Application.platform.In(RuntimePlatform.Android, RuntimePlatform.IPhonePlayer))
            {
                foreach (var t in Input.touches)
                {
                    pos = t.position;
                    Ray inputRay = cam.ScreenPointToRay(pos);
                    if (Physics.Raycast(inputRay.origin, inputRay.direction, out hit, Mathf.Infinity, uiMask))
                        return true;
                }
                return false;
            }
            else
            {
                pos = Input.mousePosition;
                Ray inputRay = cam.ScreenPointToRay(pos);
                return Physics.Raycast(inputRay.origin, inputRay.direction, out hit, Mathf.Infinity, uiMask);
            }

        }
    }

}