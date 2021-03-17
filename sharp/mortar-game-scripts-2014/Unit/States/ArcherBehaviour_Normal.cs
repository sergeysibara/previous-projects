using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Pathfinding;
using Pathfinding.RVO;

public class ArcherBehaviour_Normal : BaseUnitBehaviour
{
    private readonly List<Checkpoint> _traversedCheckpoints = new List<Checkpoint>();

    private void Start()
    {
    }

    private bool _hasPointForMoving;

    private void Update()
    {
        if (Conditions.Unit.CanAttackFromPosition(gameObject, BattleManager.GetPlayer(), ((ArcherStats)_mediator.Stats).AttackDistance, transform.position))
        {
            //DebugUtils.DrawVerticalRay(transform.position,30,Color.red);
            _mediator.SetState(UnitState.Attack);
            _hasPointForMoving = false;
        }
        else if (Conditions.GameBounds.IsCloserThanPlayerShootBounds(transform.position))
        {
            if (!_hasPointForMoving)
            {
                Vector3 newPos = RandomUtils.PointInsideCircle(transform.position, 10, Consts.LayerMasks.GroundForUnits);
                if (Conditions.Unit.CanMoveAndAttackFromPosition(gameObject, BattleManager.GetPlayer(), ((ArcherStats)_mediator.Stats).AttackDistance, newPos))
                {
                    //DebugUtils.DrawVerticalRay(newPos, 20, Color.magenta);

                    _mediator.RichAI.target = _mediator.FsmVariables.SecondTarget;
                    _mediator.FsmVariables.SecondTarget.position = newPos;
                    _mediator.RichAI.StartMove();
                    _hasPointForMoving = true;
                }
            }
        }
    }

    private void OnCheckpointEnter(Checkpoint checkPoint)
    {
        if (Conditions.GameBounds.IsCloserThanPlayerShootBounds(transform.position))
            return;

        Actions.AIAction.TryChangeTarget(this, _mediator.RichAI, checkPoint, _traversedCheckpoints);
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    Debug.LogWarning("UNIT ", this);
    //}
}
