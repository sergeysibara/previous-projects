using UnityEngine;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(Animation))]
public class UnitAI : BaseObjectAI
{
    #region Inspector Variables

    [SerializeField]
    protected float _speed = 3.0f;

    [SerializeField]
    protected float _rotationSpeed = 5.0f;

    [SerializeField]
    protected float _angleAtWhichActionIsAllowed = 45f;

    public float TargetFindingInterval = 0.2f;
    public float PathFindingInterval = 0.2f;
    public float NextWaypointDistance = 0.3f;

    #endregion


    public Seeker Seeker;
    public UnitAnimation UnitAnimation { get; private set; }
    public UnitState State { get { return _state; } set { _state = value; } }

    [SerializeField]
    protected UnitState _state;

    protected override void Awake()
    {
        base.Awake();

        Seeker = GetComponent<Seeker>();
        UnitAnimation = new UnitAnimation(animation);
        State = UnitState.Free;
    }

    protected virtual void Update()
    {
    }

    protected virtual void FixedUpdate()
    {
        //отменяем влияние физики. Включается только когда юнит перемещается - в функции Move().
        rigidbody.isKinematic = true;
    }

    public override void Die()
    {
        base.Die();
        UnitAnimation.State = UnitAnimation.States.Die;
        enabled = false;
    }


    public override void DequeueTask()
    {
        base.DequeueTask();
        UnitAnimation.State = UnitAnimation.States.Idle;

        if (State == UnitState.CommandPerforming)
            State = UnitState.Free;
    }

    /// <summary>
    /// Проверяется - разрешено ли действие при разнице между текущим и целевым направлениями юнита
    /// </summary>
    public bool ActionIsAllowed(Vector3 targetDirection)
    {
        return (Vector3.Angle(transform.forward, targetDirection) < _angleAtWhichActionIsAllowed);
    }

    public void RotateAndMove(ref int currentWaypointIndex, Vector3 currentWaypoint, Path path)
    {
        //учитывать, что currentWaypoint не всегда равно path.vectorPath[currentWaypointIndex]

        //проверка, разрешено ли юниту двигаться к следущей точки пути, вместо текущей
        if ((currentWaypoint - transform.position).sqrMagnitude <= NextWaypointDistance * NextWaypointDistance)//(Vector3.Distance(transform.position, currentWaypoint) <= NextWaypointDistance)
        {
            if (currentWaypointIndex < path.vectorPath.Length - 1)
            {
                currentWaypointIndex++;
                currentWaypoint = path.vectorPath[currentWaypointIndex];
            }
        }

        Vector3 dirToCurrentWaypoint = new Vector3(currentWaypoint.x - transform.position.x, 0.0f, currentWaypoint.z - transform.position.z);
        if (ActionIsAllowed(dirToCurrentWaypoint))
        {
            Move(currentWaypoint);
            dirToCurrentWaypoint = new Vector3(currentWaypoint.x - transform.position.x, 0.0f, currentWaypoint.z - transform.position.z);
        }
        else
            UnitAnimation.State = UnitAnimation.States.Idle;

        Rotate(dirToCurrentWaypoint);
    }

    public void Rotate(Vector3 direction)
    {
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), _rotationSpeed * Time.fixedDeltaTime);
    }

    void Move(Vector3 currentWaypoint)
    {
        //включение физики. Чтобы юниты не проходили сквозь объекты.
        rigidbody.isKinematic = false;

        Vector3 dir = (currentWaypoint - transform.position).normalized;
        if (dir != Vector3.zero)
        {
            transform.position += (dir * Time.fixedDeltaTime * _speed);
            UnitAnimation.State = UnitAnimation.States.Run;
        }
    }

}