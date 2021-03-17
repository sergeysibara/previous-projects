using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Pathfinding;
using System.Linq;

public class WarriorBehaviour_Normal : BaseUnitBehaviour
{
    private readonly List<Checkpoint> _traversedCheckpoints = new List<Checkpoint>();

    void Start()
    {
        
    }

    private void OnCheckpointEnter(Checkpoint checkPoint)
    {
        Actions.AIAction.TryChangeTarget(this, _mediator.RichAI, checkPoint, _traversedCheckpoints);
    }

    private void OnFinalCheckpointEnter(Checkpoint checkPoint)
    {
        if (!enabled)
            return;
        EventAggregator.PublishT(GameEvent.OnPlayerDamage, this, new Damage(DamageType.Close, _mediator.Stats.AttackPower));//передать величину урона игроку, либо компонент unit.variables
        EventAggregator.PublishT(GameEvent.OnRemoveUnitFromGame, this, _mediator.Stats.SpawnRate);
        Destroy(gameObject);//убирает юнита, т.к. он дошел до форта игрока.    
    }



    //void OnTriggerEnter(Collider other)
    //{
    //    Debug.LogWarning("UNIT ", this);
    //}
}
