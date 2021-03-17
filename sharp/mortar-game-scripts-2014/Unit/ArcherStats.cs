using UnityEngine;
using System.Collections;

public class ArcherStats : UnitStats 
{
    public float AttackDistance = 50f;

    [SerializeField]
    private bool _showAttackDistance;

    private void OnDrawGizmosSelected()
    {
        if (_showAttackDistance)
        {
            Gizmos.color = Color.red.WithAlpha(0.5f);
            Gizmos.DrawSphere(transform.position, AttackDistance);
        }
    }
}
