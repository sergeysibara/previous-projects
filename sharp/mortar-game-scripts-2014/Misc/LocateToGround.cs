using UnityEngine;
using System.Collections;

public class LocateToGround : MonoBehaviour 
{
    private bool _isDoneRaycastToGround;

    void Start()
    {
        enabled = false;
    }

    private void OnDrawGizmos()
    {
        if (!_isDoneRaycastToGround)
        {
            //после остановки игры в редакторе, или при запуске редактора, устанавливает Y позицию в точке, где земля.
            transform.position = PhysicsUtils.RaycastFromUpToDown(transform.position, Consts.LayerMasks.GroundForUnits).point;
            _isDoneRaycastToGround = true;
        }
    }
}
