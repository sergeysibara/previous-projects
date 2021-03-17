using System;
using System.Collections.Generic;
using Shapes;
using UnityEngine;
using System.Collections;

public class Signal : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;

    public const float PosY = 10.32f;

    private Direction _prevOutDirection;

    private Shape _currentShape;
    private int _currentWaypointIndex = 0;
    private Vector3 _currentWaypoint;
    private List<Vector3> _path;

    private GameObject _prefab;

    /// <summary>
    /// Сигнал-предок, который клонировал текущий сигнал.
    /// </summary>
    private Signal _parentSignal;

    private bool _isClonedInCurrentShape;

    private List<Shape> _traversedShapes = new List<Shape>();

    /// <summary>
    /// Максимальное расстояние от объекта до точки пути, к которой он движется, при достижении которого он может двигаться к следующей точке.
    /// </summary>
    private float NextWaypointDistance
    {
        get
        {
            return Time.deltaTime*_speed;// *1.01f; 
        }
    }

    private void Awake()
    {
        EventMessenger.SendMessage(GameEvent.OnCreateSignal, this);

        transform.parent = SceneContainers.Signals;
        if (NodesGrid.Grid == null)
        {
            Debug.LogError("NodesGrid.Grid is not initialized");
            return;
        }
        VectorInt2 nodeIndex = transform.position;
        _currentShape = NodesGrid.Grid[nodeIndex.x, nodeIndex.y].Shape;
        TryAddShapeToList(_currentShape);

        if (_currentShape.IsInRotateProcess)
            DestroySelf();
    }

    /// <summary>
    /// Инициализация для использования вне скрипта.
    /// </summary>
    public void Init(Direction dir, GameObject prefab)
    {
        _prevOutDirection = dir;
        transform.SetY(PosY);
        _prefab = prefab;

        _path = _currentShape.GetPath(_prevOutDirection);
    }

    // Update is called once per frame
    private void Update()
    {
        if (_path == null)
        {
            return;
        }

        if (PathIsTraversed())
        {
            _path = null;
            _currentWaypointIndex = 0;

            if (!_isClonedInCurrentShape)
                _prevOutDirection = _currentShape.GetOutDirection(_prevOutDirection);//получение направления выхода из текущей shape

            var prevShape = _currentShape;
            _currentShape = NodesGrid.GetNextShape(_currentShape, _prevOutDirection);
            if (prevShape != _currentShape)
                TryAddShapeToList(_currentShape);

            if (_currentShape == null || _currentShape.IsInRotateProcess || !_currentShape.HasConnection(_prevOutDirection))
            {
                DestroySelf();
                return;
            }

            _path = _currentShape.GetPath(_prevOutDirection);
            _isClonedInCurrentShape = false;
            _parentSignal = null;
        }

        bool isUpdated = UpdateCurrentWaypoint();
        Vector3 moveDirection = GetMoveDirection(_currentWaypoint);

        if (isUpdated)
        {
            TryClone();
            CorrectMovement(moveDirection);
        }

        Rotate(_currentWaypoint);
        Move(_currentWaypoint, moveDirection);  
    }

    private void OnTriggerEnter(Collider c)
    {
        if (gameObject.CompareTag(Consts.Tags.survivedSignal))
        {
            tag = "";
            return;
        }
        var signal2 = c.GetComponent<Signal>();
        if (signal2 != null && signal2._parentSignal != this && _parentSignal != signal2)
        {
            signal2.tag = Consts.Tags.survivedSignal;
            DestroySelf();
        }
    }

    private void TryAddShapeToList(Shape currentShape)
    {
        if (_traversedShapes.Contains(currentShape))
            DestroySelf();
        _traversedShapes.Add(currentShape);
    }

    private void TryClone()
    {
        if (!SignalManager.IsAllowedCreateSignal)
            return;

        if (SignalManager.IsRandomCloning)
        {
            bool doClone = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
            if (!doClone)
                return;
        }

        TeeShape teeShape = _currentShape as TeeShape;
        if (teeShape != null && _currentWaypointIndex == 2 && _path.Count == 3)// у клонированного сигнала путь содержит только 2 точки, поэтому он не пройдет проверку:  _path.Count == 3
        {
            var signalGO = (Instantiate(_prefab, teeShape.CenterSignalPoint, new Quaternion(0, 0, 0, 0)) as GameObject);
            var signal = signalGO.GetComponent<Signal>();

            signal._path = teeShape.GetSecondPath(_prevOutDirection);
            var dir = teeShape.GetSecondOutDirection(_prevOutDirection);

            signal._prevOutDirection = dir;
            signal.transform.SetY(PosY);
            signal._isClonedInCurrentShape = true;
            signal._parentSignal = this;
            signal._prefab = _prefab;

            signal._traversedShapes.AddRange(_traversedShapes);
        }
    }

    private bool UpdateCurrentWaypoint()
    {
        bool isUpdated = false;
        Vector3 currentWaypoint = _path[_currentWaypointIndex];

        var dist = Vector3.Distance(currentWaypoint, transform.position);
        if (dist <= NextWaypointDistance)
        {
            if (_currentWaypointIndex < _path.Count - 1)
            {
                _currentWaypointIndex++;
                isUpdated = true;
            }
        }
        _currentWaypoint = _path[_currentWaypointIndex];
        return isUpdated;
    }

    /// <summary>
    /// Корректировка перемещения для сигнала в центре TeeShape.
    /// </summary>
    private void CorrectMovement(Vector3 moveDirection)//float distanceToCurrentWaypoint)
    {
        Vector3 prevWaipoint = _path[_currentWaypointIndex - 1];
        float distanceToPrevWaypoint = Vector3.Distance(prevWaipoint, transform.position);

        if (_currentShape is TeeShape && _currentWaypointIndex == 2) //проверка что сигнал в центре TeeShape
        {
            Vector3 dir = (prevWaipoint - transform.position).normalized;
            if (dir != Vector3.zero)
            {
                transform.position = prevWaipoint;
                if (moveDirection != Vector3.zero)
                    transform.position -= moveDirection * distanceToPrevWaypoint; //сдвиг в сторону, противоположную следующему перемещению.
            }
        }

        //не помешала бы еще проверка на предыдущее направление перемещения и текущее к prevWaipoint, чтобы определить не дошел сигнал до prevWaipoint или уже прошел её.
    }

    private Vector3 GetMoveDirection(Vector3 currentWaypoint)
    {
        return (currentWaypoint - transform.position).normalized;
    }

    private void Move(Vector3 currentWaypoint, Vector3 moveDirection)
    {
        if (moveDirection != Vector3.zero)
            transform.position += (moveDirection * Time.deltaTime * _speed);
    }

    private void Rotate(Vector3 currentWaypoint)
    {
        Vector3 targetPos = currentWaypoint;
        transform.LookAt(targetPos);
    }

    private bool PathIsTraversed()
    {
        if (_path.Count == 0)
            return true;
        Vector3 endWaypoint = _path[_path.Count - 1];
        return Vector3.Distance(endWaypoint, transform.position) <= NextWaypointDistance;
    }

    public void DestroySelf()
    {
        /*//чтобы не удалялся сразу, а исчезал постепенно
        ParticleSystem[] psArray = GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in psArray)
            ps.emissionRate = 0f;
        */

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        EventMessenger.SendMessage(GameEvent.OnDestroySignal, this);
    }
}
