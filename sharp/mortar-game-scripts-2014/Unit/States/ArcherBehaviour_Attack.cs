using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ArcherBehaviour_Attack : BaseUnitBehaviour
{
    [SerializeField]
    private Transform _arrowPrefab;

    [SerializeField]
    private float _attackCooldown = 5f;

    [SerializeField, TooltipAttribute("Скорость поворота в градусах в секунду")]
    private float _rotationSpeed = 150f;


    /// <summary>
    /// Чекпоинт
    /// </summary>
    private Transform _firstTarget;

    /// <summary>
    /// Цель, которую нужно атаковать
    /// </summary>
    private Transform _attackableTarget;

    private ArrowController _arrow;

    protected override void Awake() 
    {
        base.Awake();
        _attackableTarget = BattleManager.GetPlayer();
        _mediator.RichAI.OnEndMoveToTarget += OnMoveEnd;
    }

    private void OnMoveEnd()
    {
        //StopMove();
        _hasPointForMoving = false;
    }

    private void OnEnable()
    {
        _firstTarget = _mediator.RichAI.target;
        _mediator.FsmVariables.SecondTarget.position = transform.position;
        _mediator.RichAI.target = _mediator.FsmVariables.SecondTarget;

        StopMove();
        PreparationsForAttack();
    }

    /// <summary>
    /// Поворот к цели с последующей гарантированной атакой
    /// </summary>
    private void PreparationsForAttack()
    {
        _attackInProgress = true;
        _hasPointForMoving = false;
        Action attack = () =>
            {
                _remainTimeBeforeNextAttack = _attackCooldown;
                StopMove();
                _mediator.AnimatorController.StartAttack();
                _mediator.AnimatorController.StoptAttack();
            };

        var arrow=(Transform)Instantiate(_arrowPrefab, transform.position, Quaternion.LookRotation(Vector3.up));
        arrow.parent = transform;//todo появление стрелы за спиной
        _arrow = arrow.GetComponent<ArrowController>();
        //DebugUtils.DrawVerticalRay(arrow.position,30,Color.cyan);

        StartCoroutine(Actions.Position.RotateToTargetCoroutine(transform, _attackableTarget, _rotationSpeed, 10, OnComplete: attack));
    }

    //вызывается из анимации
    private void OnAttackEnd()
    {
        _attackInProgress = false;
    }

    //вызывается из анимации
    private void Shoot()
    {
        _arrow.Shoot(BattleManager.GetPlayer(), _mediator.Stats.AttackPower);
    }
    

    private float _remainTimeBeforeNextAttack;
    private bool _hasPointForMoving;
    private bool _attackInProgress;

    private void Update()
    {
        _remainTimeBeforeNextAttack -= Time.deltaTime;

        if (_attackInProgress)
            return;

        if (_remainTimeBeforeNextAttack > 0)//если кулдаун после атаки еще продолжается
        {
            if (!_hasPointForMoving)
            {
                Vector3 newPos = RandomUtils.PointInsideCircle(transform.position, 10, Consts.LayerMasks.GroundForUnits);
                if (Conditions.Unit.CanMoveAndAttackFromPosition(gameObject, _attackableTarget, ((ArcherStats) _mediator.Stats).AttackDistance, newPos))
                {
                    //DebugUtils.DrawVerticalRay(newPos, 20, Color.magenta);

                    _mediator.FsmVariables.SecondTarget.position = newPos;
                    _mediator.RichAI.StartMove();
                    _hasPointForMoving = true;
                }
            }
        }
        else //если кулдаун атаки закончился
        {
            //если может атаковать с текущей позиции
            if (Conditions.Unit.CanAttackFromPosition(gameObject, _attackableTarget, ((ArcherStats) _mediator.Stats).AttackDistance, transform.position))
            {
                StopMove();
                PreparationsForAttack();
                _hasPointForMoving = false;
            }
            else //если не может атаковать с текущей позиции и
                if (!Conditions.Unit.CanMoveAndAttackFromPosition(gameObject, _attackableTarget, ((ArcherStats) _mediator.Stats).AttackDistance, _mediator.FsmVariables.SecondTarget.position)
                    || !_hasPointForMoving)
                    //если не может атаковать с позиции, в которую перемещается, или он не перемещается, то сменить стейт
                {
                    _hasPointForMoving = false;
                    //Debug.LogWarning("SetState(UnitState.Normal",this);
                    _mediator.RichAI.target = _firstTarget;
                    _mediator.RichAI.StartMove();
                    _mediator.SetState(UnitState.Normal);
                    //Debug.Break();
                }
        }
    }

    private void StopMove()
    {
        _mediator.FsmVariables.SecondTarget.position = transform.position;
        _mediator.RichAI.StopMove(_mediator.FsmVariables.SecondTarget.position);
    }

    private void OnDestroy()
    {
        if (_mediator != null && _mediator.RichAI != null && _mediator.RichAI.OnEndMoveToTarget!=null)
            _mediator.RichAI.OnEndMoveToTarget -= OnMoveEnd;
    }

    //private void OnDrawGizmos()
    //{
    //    var rect = RTSCamera.Instance.GetBounds();
    //    rect.yMin += 15f;
    //    Gizmos.color = Color.blue.WithAlpha(0.3f);
    //    GizmosUtils.DrawRect(rect, 20, 15);
    //}

}
