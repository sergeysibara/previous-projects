using UnityEngine;
using System.Collections;
using Pathfinding;

public class FollowToTargetForAttack_Task : Task
{
    public class FollowToTargetForAttack_TaskData : ITaskData
    {
        Transform _target;
        bool _isParallel;

        public FollowToTargetForAttack_TaskData(Transform target, bool setTaskAsParallel)
        {
            _target = target;
            _isParallel = setTaskAsParallel;
        }

        public System.Type GetTaskType()
        {
            return typeof(FollowToTargetForAttack_Task);
        }

        public Transform Target
        {
            get { return _target; }
        }

        public bool IsParallel
        {
            get { return _isParallel; }
        }
    }

    const string func_UpdatePath = "UpdatePath";
    const string func_UpdateTargetPosition = "UpdateTargetPosition";

    FollowToTargetForAttack_TaskData _taskData;
    BattleUnitAI _performer;

    bool _canUpdateTargetPositionAndPath = true;
    Path _path;
    int _currentWaypointIndex;
    TargetPositionPair _targetPositionPair;

    public override void SetTaskData<T>(T data)
    {
        _taskData = data as FollowToTargetForAttack_TaskData;
        if (_taskData == null)
            Debug.LogError("TaskData is not " + typeof(FollowToTargetForAttack_TaskData) + " type");
        IsParallel = _taskData.IsParallel;//отмечаем - будет ли задача с текущими данными выполняться параллельно с default задачей или нет
    }

    public override void SetPerformer<T>(T unit)
    {
        _performer = unit as BattleUnitAI;
        if (_performer == null)
            Debug.LogError("Performer is not " + typeof(BattleUnitAI) + " type");
    }

    public override void StartTask()
    {
        if (_performer == null)
            Debug.LogError("Performer is not added");
        if (_taskData == null)
            Debug.LogError("TaskData is not added");
        if (!TargetIsExist())
        {
            CompleteTask();
            return;
        }
        
        if (!IsActive)
        {
            if (!_performer.gameObject.IsDied())
            {
                //Сброс всех вычисляемых параметров
                ClearTemporaryVariables();
                
                enabled = true;
                IsActive = true;

                if (_taskData.Target.IsUnit()) //если цель - подвижный объект, то запускаем корутину для отслеживания его позиции
                {
                    StartCoroutine(func_UpdateTargetPosition);
                }
                else //иначе ищем позицию только 1  раз
                {
                    bool isPathPossible;
                    _targetPositionPair = TargetFindingMethods.GetNearestTargetPositionToSeeker(_performer.transform.position, _taskData.Target, out isPathPossible);
                    if (!isPathPossible)
                    {
                        CompleteTask();
                        return;
                    }
                }

                StartCoroutine(func_UpdatePath);
            }
        }
    }

    public override void CompleteTask()
    {
        base.CompleteTask();
        _taskData = null;
        _performer.UnitAnimation.State = UnitAnimation.States.Idle;
        _performer.DequeueTask();            
    }

    void FixedUpdate()
    {
        if (IsParallel && _performer.State == UnitState.HasTargetInDefaultTask)
        {
            _canUpdateTargetPositionAndPath = false;
            return;
        }

        _performer.State = UnitState.CommandPerforming;
        _canUpdateTargetPositionAndPath = true; //разрешение искать более ближнюю цель 

        //Рисует линию от граничной ноды до точки, куда будет пытаться следовать юнит  
        Debug.DrawLine(_targetPositionPair.FollowPosition, _targetPositionPair.NearestBoundaryNodePosition, Color.blue);

        if (TargetIsExist())
        {
            Vector3 performerPos = _performer.transform.position;

            Vector3 closestPoint = _taskData.Target.collider.ClosestPointOnBounds(performerPos);
            float distance = Vector3.Distance(closestPoint, performerPos);

            if (distance <= _performer.AttackDistance) //если юнит на дистанции атаки 
            {
                _canUpdateTargetPositionAndPath = false;
                _performer.Attack(_taskData.Target, _targetPositionPair);
            }
            else
            {
                if (_path != null)
                {
                    //если юнит находится в ноде _targetPositionPair.FollowPosition, то следовать к NearestBoundaryNodePosition(к центру ближайшей граничной ноде цели), а не к _path.vectorPath[_currentWaypoint]
                    Vector3 currentWaypoint = _path.vectorPath[_currentWaypointIndex];
                    Vector3 endWaypoint = _path.vectorPath[_path.vectorPath.Length - 1];
                    if (AstarPath.active.GetNearest(endWaypoint).node.GetNodeIndex() == AstarPath.active.GetNearest(performerPos).node.GetNodeIndex())
                    {
                        if (Vector3.Distance(endWaypoint, performerPos) < GameManager.HalfNodeSizeSqrt)
                            currentWaypoint = _targetPositionPair.NearestBoundaryNodePosition;
                    }

                    //поворот и перемещение к текущей Waypoint
                    _performer.RotateAndMove(ref _currentWaypointIndex, currentWaypoint, _path);
                }
                else  //если пути нет
                {
                    _performer.UnitAnimation.State = UnitAnimation.States.Idle;
                }
            }
        }
        else //если цели нет
        {
            _performer.UnitAnimation.State = UnitAnimation.States.Idle;
            CompleteTask();
        }
    }

    IEnumerator UpdateTargetPosition()
    {
        while (true)
        {
            if (_canUpdateTargetPositionAndPath)
            {
                if (TargetIsExist())
                {
                    bool isPathPossible;
                    _targetPositionPair = TargetFindingMethods.GetNearestTargetPositionToSeeker(_performer.transform.position, _taskData.Target, out isPathPossible);
                    if (!isPathPossible)
                    {
                        CompleteTask();
                        yield break;
                    }
                }
                else
                {
                    CompleteTask();
                    yield break;
                }
            }
            yield return new WaitForSeconds(_performer.TargetFindingInterval);
        }
    }

    IEnumerator UpdatePath()
    {
        while (true)
        {
            if (_canUpdateTargetPositionAndPath)
            {  
                _performer.Seeker.StartPath(_performer.transform.position, _targetPositionPair.FollowPosition, OnPathComplete);
            }
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

    bool TargetIsExist()
    {
        if (_taskData.Target == null)
        {
            _path = null;
            return false;
        }
        else
            if (_taskData.Target.gameObject.IsDied())
            {
                _path = null;
                return false;
            }
        return true;
    }

    protected override void ClearTemporaryVariables()
    {
        _path = null;
    }

}
