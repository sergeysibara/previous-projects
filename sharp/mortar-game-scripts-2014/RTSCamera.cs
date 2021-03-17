using System;
using System.Collections;
using UnityEngine;

public class RTSCamera : RequiredMonoSingleton<RTSCamera>
{
    [SerializeField]
    private Vector2 _speedFactor = new Vector2(1f, 1f);

    [SerializeField]
    private Vector2 _speedLimit = new Vector2(1f, 1f);

    [SerializeField]
    private Vector2 _boundsX = new Vector2(50f, 150);

    [SerializeField]
    private Vector2 _boundsY = new Vector2(20, 100);

    [SerializeField]
    private float _positionAboveGround = 20f;

    public Rect GetBounds()
    {
        return new Rect { xMin = _boundsX[0], xMax = _boundsX[1], yMin = _boundsY[0], yMax = _boundsY[1] };
    }



    protected void Start()
    {        
        _newPos = transform.position;
        UpdatePositionAboveGround();
        transform.position = _newPos;

        if (Application.platform.In(RuntimePlatform.Android, RuntimePlatform.IPhonePlayer))
        {
            _speedFactor *= 1.8f;
        }
    }


    private float axelX=1;
    private float axelY=1;

    private void UpdateAxel()
    {
        var x = Input.GetAxis("Mouse X");
       // Debug.LogWarning(x);

        if (Mathf.Abs(x) < 0.5f)
            axelX = 0.5f;
        else
        {
            axelX = 1f ;
        }

        axelX = Mathf.Clamp(axelX, 0.3f, 1);



        var y = Input.GetAxis("Mouse Y");
        //Debug.LogWarning(x);

        if (Mathf.Abs(y) < 0.5f)
            axelY = 0.5f;
        else
        {
            axelY =1f;
        }
        axelY = Mathf.Clamp(axelY, 0.3f, 1);
    }

    //перемещение в FixedUpdate, т.к. более плавно выгл€дит и не зависит от fps
    void FixedUpdate()
    {
        if (BattleManager.Instance.Pause)
            return;
        
        if (Input.GetMouseButton(1))
            return;

        if (Input.touchCount>0)
            if (Input.GetTouch(0).phase != TouchPhase.Moved)
                return;

        //UpdateAxel();
        float translationX = Mathf.Clamp(Input.GetAxis("Mouse X") * _speedFactor.x * axelX, -_speedLimit.x, _speedLimit.x);
        float translationZ = Mathf.Clamp(Input.GetAxis("Mouse Y") * _speedFactor.y * axelY, -_speedLimit.y, _speedLimit.y);

        Vector3 newPos = transform.position;

        Vector3 desiredPos = transform.position;
        desiredPos.x = Mathf.Clamp(desiredPos.x + translationX, _boundsX[0], _boundsX[1]);
        if (CheckGround(desiredPos))
            newPos.x = desiredPos.x;

        desiredPos = transform.position;
        desiredPos.z = Mathf.Clamp(desiredPos.z + translationZ, _boundsY[0], _boundsY[1]);
        if (CheckGround(desiredPos))
            newPos.z = desiredPos.z;

        //transform.position = newPos;
        _newPos = newPos;
        UpdatePositionAboveGround();
        //if (FPSCounter.Instance.GetFPS()<30)
        transform.position = _newPos;
    }

    private Vector3 _newPos;
    private void Update()
    {
        transform.position = _newPos;
    }

    private void UpdatePositionAboveGround()
    {
        RaycastHit hit = PhysicsUtils.RaycastFromUpToDown(_newPos, Consts.LayerMasks.GroundForCamera);
        if (hit.collider != null)
        {
            var height = PhysicsUtils.RaycastHeigthFromUpToDown - hit.distance + _positionAboveGround;
            _newPos.y = height;
            //transform.SetY(height);
        }
        else
            Debug.LogWarning("Camera position is not over ground", this);
    }


    private bool CheckGround(Vector3 testPosition)
    {
        return PhysicsUtils.RaycastFromUpToDown(transform.position, Consts.LayerMasks.GroundForCamera).collider!=null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red.WithAlpha(0.3f);
        GizmosUtils.DrawRect(GetBounds(), 20, 15);
    }
}
