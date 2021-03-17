using UnityEngine;
using System.Collections;
using Pathfinding;

public class FollowToPoint_Task : Task
{
    public class FollowToPoint_TaskData : ITaskData
    {
        Vector3 _followPoint;

        public FollowToPoint_TaskData(Vector3 point)
        {
            _followPoint = point;
        }

        public System.Type GetTaskType()
        {
            return typeof(FollowToPoint_Task);
        }

        public Vector3 GetFollowPoint()
        {
            return _followPoint;
        }
    }

    const string func_UpdatePath = "UpdatePath";

    FollowToPoint_TaskData _taskData;
    UnitAI _performer;
    Path _path;
    int _currentWaypointIndex;


    public override void SetTaskData<T>(T data)
    {
        _taskData = data as FollowToPoint_TaskData;
        if (_taskData == null)
            Debug.LogError("TaskData is not " + typeof(FollowToPoint_TaskData) + " type", GetComponent<Transform>());
    }

    public override void SetPerformer<T>(T unit)
    {
        _performer = unit as UnitAI;
        if (_performer == null)
            Debug.LogError("Performer is not " + typeof(UnitAI) + " type", unit);
    }

    public override void StartTask()
    {
        if (_performer == null)
            Debug.LogError("Performer is not added", GetComponent<Transform>());
        if (_taskData == null)
            Debug.LogError("TaskData is not added", GetComponent<Transform>());

        if (!IsActive)
        {
            if (!_performer.gameObject.IsDied())
            {
                //Сброс вычисляемых параметров
                ClearTemporaryVariables();

                enabled = true;
                IsActive = true;

                _performer.State = UnitState.CommandPerforming;
                StartCoroutine(func_UpdatePath);
            }
        }
    }

    public override void CompleteTask()
    {
        base.CompleteTask();
        _taskData = null;
        _performer.DequeueTask();
    }

    void FixedUpdate()
    {
        if (_path != null)
        {
            //проверка-достигнут ли конец пути
            Vector3 currentWaypoint = _path.vectorPath[_currentWaypointIndex];
            Vector3 endWaypoint = _path.vectorPath[_path.vectorPath.Length - 1];
            if (Vector3.Distance(endWaypoint, _performer.transform.position) <= _performer.NextWaypointDistance)
            {
                _performer.UnitAnimation.State = UnitAnimation.States.Idle;
                CompleteTask();
                return;
            }

            //поворот и перемещение к текущей Waypoint
            _performer.RotateAndMove(ref _currentWaypointIndex, currentWaypoint, _path);
        }
        else  //если пути нет
            _performer.UnitAnimation.State = UnitAnimation.States.Idle;
    }

    IEnumerator UpdatePath()
    {
        while (true)
        {
            _performer.Seeker.StartPath(_performer.transform.position, _taskData.GetFollowPoint(), OnPathComplete);
            yield return new WaitForSeconds(_performer.PathFindingInterval);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            _path = p;
            _currentWaypointIndex = 1;
        }
        else
        {
            _path = null;
            CompleteTask();
        }
    }

    protected override void ClearTemporaryVariables()
    {
        _path = null;
    }

}
