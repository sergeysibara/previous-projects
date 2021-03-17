using System;
using UnityEngine;

public class DirectMovementController : MonoBehaviour, ISimpleMovement
{
    [SerializeField]
    private float _speed = 1f;

    private Action _onPathTraversed;

    private Transform _target;

    /// <summary>
    /// Максимальное расстояние от объекта до точки пути, к которой он движется, при достижении которого он может двигаться к следующей точке. Использовать только в FixedUpdate().
    /// </summary>
    private float NextWaypointDistance
    {
        get { return Time.fixedDeltaTime * _speed* 1.01f; }
    }

    private void Awake()
    {     
    }

    private void FixedUpdate()
    {
        if (_target != null)
        {
            Vector3 targetPos = _target.position;
            targetPos.y = transform.position.y;
            //проверка-достигнут ли конец пути
            if (Vector3.Distance(targetPos, transform.position) <= NextWaypointDistance)
            {
                _target = null;
                _onPathTraversed();
                return;
            }
            Rotate(targetPos);
            Move(targetPos);            
        }
    }

    public void StartMovement(Transform target, Action onPathTraversed)
    {
        if (target == null)
        {
            Debug.LogWarning("target=null", this);
            return;
        }

        _target = target;
        _onPathTraversed = onPathTraversed;
    }

    ///<param name="currentWaypoint">Ближайшая точка пути, к которой движется seeker</param>
    private void Move(Vector3 currentWaypoint)
    {
        Vector3 dir = (currentWaypoint - transform.position).normalized;
        if (dir != Vector3.zero)
        {
            transform.position += (dir * Time.fixedDeltaTime * _speed);
        }
    }

    private void Rotate(Vector3 currentWaypoint)
    {
        transform.LookAt(currentWaypoint);
    }
}

