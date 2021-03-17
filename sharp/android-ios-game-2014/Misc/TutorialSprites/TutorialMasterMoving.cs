using UnityEngine;
using System.Collections;

public class TutorialMasterMoving : MonoBehaviour {

    /// <summary>
    /// Позиция назначения. В локальных координатах. Перед использованием нужно трансформировать в глобальные.
    /// </summary>
    [SerializeField]
    private Vector3 _destinationPosition;

    //[SerializeField]
    //private float _speed = 1f;

    [SerializeField]
    private float _time = 2f;

    private bool _isAllowedMoving;
    private bool _isInDestinationPosition;
    private Vector3 _globalDestinationPosition;

    private float _pathDistance;

    private float Speed
    {
        get { return _pathDistance*Time.fixedDeltaTime/_time; }
    }

    private float NextWaypointDistance
    {
        get { return Time.fixedDeltaTime * Speed * 1.01f; }
    }

	// Use this for initialization
	void Start ()
	{
        _globalDestinationPosition = transform.TransformPoint(_destinationPosition);
	    _isAllowedMoving = true;
	    _pathDistance = Vector3.Distance(transform.position, _globalDestinationPosition);
        //Debug.LogWarning(_pathDistance);
	}

    private void FixedUpdate()
    {
        if (_isInDestinationPosition || !_isAllowedMoving)
            return;

        if (Vector3.Distance(_globalDestinationPosition, transform.position) <= NextWaypointDistance)
        {
            transform.position = _globalDestinationPosition;
            _isInDestinationPosition = true;
            ShowText();
        }
        else
        {
            Move(_globalDestinationPosition);
        }
    }

    private void StartWalk()
    {
        _isAllowedMoving = true;
    }

    private void Move(Vector3 currentWaypoint)
    {
        Vector3 dir = (currentWaypoint - transform.position).normalized;
        if (dir != Vector3.zero)
        {
            transform.position += (dir * Speed);
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

    private void ShowText()
    {
        var textSprite = transform.GetComponentInChildren<SpriteAlphaChanger>();
        textSprite.StartAlphaChanging();
    }
}
