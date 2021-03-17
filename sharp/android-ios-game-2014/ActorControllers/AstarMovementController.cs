using System;
using Pathfinding;
using UnityEngine;
using System.Collections;
using System.Linq;

public class AstarMovementController : MonoBehaviour, IPathFinderMovement
{
    [SerializeField, ReadOnlyInInspector]
    private float _speed = 1f;

    private float _rndMinSpeed = 1.4f;
    private float _rndMaxSpeed = 1.9f;


    private Seeker _seeker;
    private Vector3[] _vectorPath;
    private int _currentWaypointIndex;
    private Vector3 _currentWaypoint;
    private Action _onPathTraversed;
    private Transform _target;

    public Action OnWaypointChange { set; private get; }
    /// <summary>
    /// Максимальное расстояние от объекта до точки пути, к которой он движется, при достижении которого он может двигаться к следующей точке. Использовать только в FixedUpdate().
    /// </summary>
    private float NextWaypointDistance
    {
        get { return Time.fixedDeltaTime * _speed * 1.01f; }
    }

    // Use this for initialization
    private void Awake()
    {
        _seeker = GetComponent<Seeker>();
        if (Application.loadedLevelName.Contains("Room"))
        {
            int roomNumber = int.Parse(Application.loadedLevelName.Remove(0, 4));
            _speed = 0.9f + roomNumber * 0.1f;
        }
        else
        {
            _speed = UnityEngine.Random.Range(_rndMinSpeed, _rndMaxSpeed);
            Debug.Log(_speed);
        }
        Debug.Log(_speed);
    }

    private void FixedUpdate()
    {
        if (_vectorPath != null && _target != null)
        {
            //проверка-достигнут ли конец пути
            if (Vector3.Distance(_vectorPath[_vectorPath.Length-1], transform.position) <= NextWaypointDistance)
            {
                _vectorPath = null;
                _onPathTraversed();
                return;
            }

            //поворот и перемещение к текущей Waypoint
            UpdateCurrentWaypoint();
            Rotate(_currentWaypoint);
            Move(_currentWaypoint);
        }
    }

    public void StartMovement(Transform target, Action onMovementStart, Action onPathTraversed)
    {
        _target = target;
        if (target == null)
        {
            Debug.LogWarning("target=null", this);
            return;
        }
        
        _onPathTraversed = onPathTraversed;
        _seeker.StartPath(transform.position, target.position, (p) =>
            {
                OnPathComplete(p);
                onMovementStart();
            });
    }

    public void CancelMovement()
    {
        _vectorPath = null;
        _target = null;
    }

    private void UpdateCurrentWaypoint()
    {
        Vector3 currentWaypoint = _vectorPath[_currentWaypointIndex];
        if (Vector3.Distance(currentWaypoint, transform.position) <= NextWaypointDistance) //проверка, разрешено ли двигаться к следущей точки пути, вместо текущей
        {
            if (_currentWaypointIndex < _vectorPath.Length - 1)
            {                
                if (OnWaypointChange != null)
                    OnWaypointChange();
                _currentWaypointIndex++;                
            }
        }
        _currentWaypoint = _vectorPath[_currentWaypointIndex];
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

    private void OnPathComplete(Path p)
    {
        if (_target==null)
            return;
        
        if (!p.error)
        {
            //установка высоты, как у seeker-a
            _vectorPath = p.vectorPath.Select(v => new Vector3(v.x, transform.position.y, v.z)).ToArray<Vector3>();

            _currentWaypointIndex = 1;
        }
        else
        {
            _vectorPath = null;
        }
    }
}
