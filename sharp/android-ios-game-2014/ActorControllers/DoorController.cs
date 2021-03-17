using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [SerializeField]
    private Transform pivotTransform;

    [SerializeField]
    private float angle;

    [SerializeField]
    private float timeAction;

    private bool isOpen = false;
    private float stepAngle;
    private float angleCounter = 0;

    private void Start()
    {
        EventMessenger.Subscribe(GameEvent.InvokeAdjuster, this, OpenDoor);
    }

    private void Update()
    {
        if (stepAngle != 0)
        {
            transform.RotateAround(pivotTransform.position, new Vector3(0, 1, 0), stepAngle);
            angleCounter += stepAngle;
            if (stepAngle > 0)
            {
                if (angleCounter >= angle)
                {
                    stepAngle = 0;
                    angleCounter = 0;
                }
            }
            if (stepAngle < 0)
            {
                if (angleCounter <= -angle)
                {
                    stepAngle = 0;
                    angleCounter = 0;
                }
            }
        }
    }

    private void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            stepAngle = -angle/timeAction*Time.deltaTime;
            angleCounter = 0;
        }
        //else
        //{
        //    CloseDoor();
        //}
    }

    //public void CloseDoor()
    //{
    //    if (isOpen)
    //    {
    //        stepAngle = angle/timeAction*Time.deltaTime;
    //        angleCounter = 0;
    //        isOpen = false;
    //    }
    //}
}
