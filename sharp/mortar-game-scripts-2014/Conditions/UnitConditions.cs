using UnityEngine;
using System.Collections;

namespace Conditions
{
    public static class Unit
    {
        public static bool IsAllowableDistanceForAttack(Vector3 fromPosition, Vector3 targetPosition, float unitAttackDistance)
        {
            return (Vector3.Distance(fromPosition, targetPosition) <= unitAttackDistance);
        }

        public static bool CanAttackFromPosition(GameObject self, Transform attackableTarget, float unitAttackDistance, Vector3 position)
        {
            return IsAllowableDistanceForAttack(position, attackableTarget.position, unitAttackDistance) &&
                   GameBounds.IsInsidePlayerShootBounds(position);
        }
        public static bool CanMoveAndAttackFromPosition(GameObject self, Transform attackableTarget, float unitAttackDistance, Vector3 position)
        {
            return IsAllowableDistanceForAttack(position, attackableTarget.position, unitAttackDistance) &&
                   PlacementUtils.IsFreePosition(position, 0.6f, Consts.LayerMasks.Units.AddToMask(Consts.Layers.Obstacle), self) &&
                   GameBounds.IsInsidePlayerShootBounds(position);
        }

    }

}
