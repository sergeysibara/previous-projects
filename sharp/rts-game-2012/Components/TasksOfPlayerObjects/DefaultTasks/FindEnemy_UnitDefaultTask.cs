using UnityEngine;
using System.Collections;
using Pathfinding;

public class FindEnemy_UnitDefaultTask : DefaultTask
{
    const string func_UpdateTarget = "UpdateTarget";
    const string func_UpdatePath = "UpdatePath";

    BattleUnitAI _performer;
    Path _path;
    int _currentWaypointIndex;

    bool _canUpdateTargetAndPath = true;
    Transform _target;
    TargetPositionPair _targetPositionPair;

    public override void SetPerformer<T>(T unit)
    {
        _performer = unit as BattleUnitAI;
        if (_performer == null)
            Debug.LogError("Performer is not " + typeof(BattleUnitAI) + " type");
    }

    public override void StartTask()
    {
        if (!IsActive)
        {
            if (_performer == null)
                Debug.LogError("Performer is not added");

            if (!_performer.gameObject.IsDied())
            {
                //—брос вычисл€емых параметров
                ClearTemporaryVariables();

                enabled = true;
                IsActive = true;
                StartCoroutine(func_UpdateTarget);
                StartCoroutine(func_UpdatePath);
            }
        }
    }

    void FixedUpdate()
    {
        _canUpdateTargetAndPath = true; //разрешение искать более ближнюю цель 

        //–исует линию от граничной ноды до точки, куда будет следовать юнит  
        Debug.DrawLine(_targetPositionPair.FollowPosition, _targetPositionPair.NearestBoundaryNodePosition, Color.blue);

        if (TargetIsExist())
        {
            Vector3 performerPos = _performer.transform.position;

            Vector3 closestPoint = _target.collider.ClosestPointOnBounds(performerPos);
            float distance = Vector3.Distance(closestPoint, performerPos);

            if (distance <= _performer.VisionDistance)
            {
                if (distance <= _performer.AttackDistance) //если юнит на дистанции атаки 
                {
                    _canUpdateTargetAndPath = false;
                    _performer.Attack(_target, _targetPositionPair);
                }
                else
                {
                    if (_path != null)
                    {
                        //если юнит находитс€ в ноде _targetPositionPair.FollowPosition, то следовать к NearestBoundaryNodePosition(к центру ближайшей граничной ноды цели), а не к _path.vectorPath[_currentWaypoint]
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
                        _performer.UnitAnimation.State = UnitAnimation.States.Idle;
                }
            }
            else //если цель вне радиуса видимости юнита, то прекращаем следовать за ней
            {
                ClearTemporaryVariables();
                _target = null;
                _performer.UnitAnimation.State = UnitAnimation.States.Idle;
            }
        }
    }

    IEnumerator UpdateTarget()
    {
        while (true)
        {
            if (_canUpdateTargetAndPath)
            {
                Transform prevTarget = _target;
                _target = TargetFindingMethods.FindNearestTarget(_performer.transform.position, _performer.VisionDistance, _performer.EnemyLayerMask, out _targetPositionPair);

                if (TargetIsExist()) //если есть цель
                {
                    PauseParallelTask();
                    _performer.State = UnitState.HasTargetInDefaultTask;

                    if (_target != prevTarget) //если цель изменилась, то очищаем переменную пути. Ќа тот случай, если путь не успел обновитьс€ и юниты не бежали по старому пути
                        _path = null;
                }
                else //если цели нет
                {
                    _performer.UnitAnimation.State = UnitAnimation.States.Idle;

                    if (_performer.State != UnitState.CommandPerforming)
                        _performer.State = UnitState.Free;

                    StartParallelTask();
                }
            }
            yield return new WaitForSeconds(_performer.TargetFindingInterval);
        }
    }

    IEnumerator UpdatePath()
    {
        while (true)
        {
            if (_canUpdateTargetAndPath)
            {
                if (_target != null)
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
            _path = null;
    }

    bool TargetIsExist()
    {
        if (_target == null)
        {
            _path = null;
            return false;
        }
        else
            if (_target.gameObject.IsDied())
            {
                _target = null;
                _path = null;
                return false;
            }
        return true;
    }

    protected override void StartParallelTask()
    {
        var task = _performer.GetCurrentTask();
        if (task != null && task.IsParallel && !task.IsActive)
            task.StartTask();
    }

    protected override void PauseParallelTask()
    {
        var task = _performer.GetCurrentTask();
        if (task != null && task.IsParallel && task.IsActive)
            task.PauseTask();
    }

    protected override void ClearTemporaryVariables()
    {
        _path = null; 
    }
}
