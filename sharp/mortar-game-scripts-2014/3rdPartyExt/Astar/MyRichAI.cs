using System;
using Pathfinding;
using UnityEngine;
using System.Collections;

public class MyRichAI : RichAI
{
    public Action OnEndMoveToTarget;

    public bool IsStoped { get; private set; }

    public void StopMove(Vector3 stopPos)
    {
        if (IsStoped)
            return;
        target.position = stopPos;
        repeatedlySearchPaths = false;
        IsStoped = true;
        UpdatePath();

        // либо //RichAI.repeatedlySearchPaths = false;//остановка перемещения
        //seeker.pathCallback -= OnPathComplete;            
        //Debug.LogWarning("stopmove");
    }

    public void StartMove()
    {
        if (!IsStoped)
            return;
        repeatedlySearchPaths = true;
        IsStoped = false;
        //Debug.LogWarning("startmove");
        //seeker.pathCallback += OnPathComplete;
    }


    /** Called when the end of the path is reached */
    protected override void OnTargetReached()
    {
        if (OnEndMoveToTarget != null)
            OnEndMoveToTarget();
        //Debug.LogWarning("OnTargerReached");
    }
}
