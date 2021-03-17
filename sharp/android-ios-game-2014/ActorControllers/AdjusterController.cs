using UnityEngine;
using System.Collections;
using System.Linq;

public class AdjusterController : MonoBehaviour 
{
    /// <summary>
    /// Позиция назначения. В локальных координатах. Перед использованием нужно трансформировать в глобальные.
    /// </summary>
    [SerializeField]    
    private Vector3 _destinationPosition;

    [SerializeField]
    private float _speed = 1f;

    private bool _isAllowedMoving;
    private bool _isInDestinationPosition;
    private Vector3 _globalDestinationPosition;
    private Animator _animator;

    /// <summary>
    /// Максимальное расстояние от объекта до точки пути, к которой он движется, при достижении которого он может двигаться к следующей точке. Использовать только в FixedUpdate().
    /// </summary>
    private float NextWaypointDistance
    {
        get { return Time.fixedDeltaTime * _speed * 1.01f; }
    }

    private void Start()
    {
        EventMessenger.Subscribe(GameEvent.InvokeAdjuster, this, StartWalk);
        _animator = GetComponent<Animator>();
        _globalDestinationPosition = transform.TransformPoint(_destinationPosition);
    }

    private void FixedUpdate()
    {
        if (_isInDestinationPosition || !_isAllowedMoving)
            return;

        if (Vector3.Distance(_globalDestinationPosition, transform.position) <= NextWaypointDistance)
        {
            transform.position = _globalDestinationPosition;
            _isInDestinationPosition = true;            
            transform.localScale = new Vector3(1.5f, 1.5f, 1);
            _animator.SetTrigger("OkTrigger");
        }
        else
        {
            Move(_globalDestinationPosition);
        }
    }

    private void StartWalk()
    {
        renderer.enabled = true;
        _animator.enabled = true;
        _isAllowedMoving = true;
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

    //отрисовка _destinationPosition. Для работы, нужно, чтобы скрипт в инспекторе был развернут 
    void OnDrawGizmosSelected()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            Gizmos.color = new Color(0.0f, 0.5f, 1.0f, 0.5f);
            var tr = GetComponent<Transform>(); //GetComponent вместо transform - чтобы не было ошибки в редакторе
            float radius = 0.15f;

            Vector3 pos = tr.TransformPoint(_destinationPosition);
            pos.y += radius;

            Gizmos.DrawSphere(pos, radius);
        }
    }
}
