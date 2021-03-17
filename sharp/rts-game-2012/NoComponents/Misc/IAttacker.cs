using UnityEngine;

public interface IAttacker
{
    LayerMask EnemyLayerMask { get; set; }
    float AttackDistance { get; }
    void Attack(Transform target, TargetPositionPair targetPositionPair);
}
