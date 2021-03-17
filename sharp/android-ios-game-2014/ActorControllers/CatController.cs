using UnityEngine;
using System.Collections;
using System.Linq;

public class CatController : MonoBehaviour
{
    private enum ActivityType
    {
        None,
        DirectMovement,
        PathFinderMovement
    }

    private enum CatAnimState
    {
        Sit = 0,
        Move = 1
    }

    [SerializeField]
    private float _firstWaitInterval = 3f;
    
    [SerializeField]
    private float _waitIntervalAfterWalk = 3f;

    [SerializeField]
    private float _waitIntervalAfterClick = 7f;

    [SerializeField]
    private float _rndMinInterval = 2f;

    [SerializeField]
    private float _rndMaxInterval = 5f;

    private IPathFinderMovement _pathFinderMovement;
    private ISimpleMovement _directMovement;
    private RandomPlayAudio _rndPlayAudio;

    private int _lastTargetIndex;
    private Animator _animator;

    private ActivityType _currentActivityType;
    private bool _isStoppedAnyActivity;

    private float _waitTime;

    private Transform[] _targets;

    [SerializeField]
    private Transform[] _tutorialTargets;

    [SerializeField]
    private Transform _tutorialTargetAfterClick;


    private void Start()
    {
        EventMessenger.Subscribe(GameEvent.StartGameProcess, this, OnStartGameProcess);
        EventMessenger.Subscribe(GameEvent.InvokeAdjuster, this, StopAnyActivity);
        EventMessenger.Subscribe(GameEvent.EngGameProcess, this, StopAnyActivity);

        _pathFinderMovement = this.GetInterfaceComponent<IPathFinderMovement>();
        if (_pathFinderMovement == null)
        {
            Debug.LogError("_pathFinderMovement=null", this);
            return;
        }
        _pathFinderMovement.OnWaypointChange = RotateShape;

        _directMovement = this.GetInterfaceComponent<ISimpleMovement>();
        if (_directMovement == null)
        {
            Debug.LogError("_directMovement=null", this);
            return;
        }

        _animator = GetComponentInChildren<Animator>();
        if (_animator == null)
        {
            Debug.LogError("_animator=null", this);
            return;
        }

        _rndPlayAudio = GetComponentInChildren<RandomPlayAudio>();
        if (_rndPlayAudio == null)
        {
            Debug.LogError("_rndPlayAudio=null", this);
            return;
        }

        _waitTime = 180;
    }

    private void Update()
    {        
        if (_waitTime > 0)
        {
            _waitTime -= Time.deltaTime;
            return;
        }

        if (!_isStoppedAnyActivity && _currentActivityType == ActivityType.None)
        {
            StartWalk();
        }
    }

    private void OnStartGameProcess()
    {
        if (Application.loadedLevelName != Consts.SceneNames.Tutorial3.ToString())
            _targets = FindObjectsOfType<SeekerTarget>().Where(c => c.enabled == true).Select(t => t.transform).ToArray();
        else
            _targets = _tutorialTargets;


        //установка ближашей цели в качестве стартовой позиции для исключения при следующем поиске целей 
        if (Application.loadedLevelName != Consts.SceneNames.Tutorial3.ToString())
            _lastTargetIndex = GetNearestTargetIndex();
        else
            _lastTargetIndex = 0;

        _waitTime = _firstWaitInterval;
    }

    /// <summary>
    /// Завершение текущего действия и затем прекращение любой дальнейшей деятельности.  
    /// </summary>
    private void StopAnyActivity()
    {
        if (!_isStoppedAnyActivity && _currentActivityType == ActivityType.PathFinderMovement)
        {
            _pathFinderMovement.CancelMovement();
            StartDirectionMovement();
        }
        _isStoppedAnyActivity = true;
    }

    private void OnClick()
    {
        if (_isStoppedAnyActivity || _currentActivityType == ActivityType.DirectMovement || _waitTime > 0)
            return;

        if (_currentActivityType == ActivityType.PathFinderMovement)
            _pathFinderMovement.CancelMovement();

        StartDirectionMovement();
    }

    private void StartWalk()
    {
        _currentActivityType = ActivityType.PathFinderMovement;
        var currentTarget = SelectTarget();
        _pathFinderMovement.StartMovement(currentTarget, () => _animator.SetInteger("state", (int) CatAnimState.Move),
                                          () =>
                                              {
                                                  RotateShape();
                                                  StopWalk();
                                              });
    }

    private void StopWalk()
    {
        if (_animator.GetInteger("state") == (int)CatAnimState.Move)
        {
            _animator.SetInteger("state", (int) CatAnimState.Sit);
        }
        _currentActivityType = ActivityType.None;

        if (Application.loadedLevelName == Consts.SceneNames.Level1.ToString())
            _waitTime = Random.Range(_rndMinInterval,_rndMaxInterval);
        else
            _waitTime = _waitIntervalAfterWalk;
        //Debug.LogWarning(_waitTime);
    }

    private void StartDirectionMovement()
    {
        _rndPlayAudio.Play();
        _currentActivityType = ActivityType.DirectMovement;
        _animator.SetInteger("state", (int)CatAnimState.Move);
        
        Transform currentTarget;
        if (Application.loadedLevelName != Consts.SceneNames.Tutorial3.ToString())
            currentTarget=SelectTarget();
        else
            currentTarget = _tutorialTargetAfterClick;

        _directMovement.StartMovement(currentTarget, () =>
            {
                StopWalk();
                if (Application.loadedLevelName == Consts.SceneNames.Level1.ToString())
                    _waitTime = Random.Range(_rndMinInterval, _rndMaxInterval);
                else
                    _waitTime = _waitIntervalAfterClick;
                //Debug.LogWarning(_waitTime);
            });
    }

    private void RotateShape()
    {
        if (_isStoppedAnyActivity || _currentActivityType == ActivityType.DirectMovement || _waitTime > 0)
            return;

        VectorInt2 nodeIndex = transform.position;
        var shape = NodesGrid.Grid[nodeIndex.x, nodeIndex.y].Shape;
        if (shape != null)
        {
            shape.RotateCommand();
        }
    }

    private Transform SelectTarget()
    {
        if (Application.loadedLevelName != Consts.SceneNames.Tutorial3.ToString())
            _lastTargetIndex = RandomUtils.RangeWithExclude(0, _targets.Length, _lastTargetIndex);
        else
        {
            _lastTargetIndex++;
            if (_lastTargetIndex > 3)
                _lastTargetIndex = 0;
        }
        return _targets[_lastTargetIndex];
    }

    private int GetNearestTargetIndex()
    {
        if (_targets.Length == 0)
        {
            Debug.LogError("targerts not found");
            return 0;
        }

        int nearestTargetIndex = 0;
        float minDist = Vector3.Distance(transform.position, _targets[0].position);
        for (int i = 1; i < _targets.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, _targets[i].position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestTargetIndex = i;
            }
        }
        return nearestTargetIndex;
    }

}
