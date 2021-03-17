using UnityEngine;
using System.Collections;

public class BattleUnitAI : UnitAI, IAttacker
{
    #region Inspector Variables
  
    [SerializeField]
    protected float _attackInterval = 2.0f;

    [SerializeField]
    protected float _attackDistance = 0.51f;

    [SerializeField]
    protected int _damage;

    [SerializeField]
    protected float _timeBeforeDamageCausing = 0.6f;

    #endregion


    public LayerMask EnemyLayerMask { get; set; }

    public float AttackDistance 
    { 
        get { return _attackDistance; } 
        protected set { _attackDistance = value; } 
    }

    protected float _lastAttackTime=-100.0f; //�����, ��������� � ��������� ����� 

    public virtual void Attack(Transform target, TargetPositionPair targetPositionPair)
    {
        //������� � ����
        Vector3 dirToTarget;
        if (target.IsBuilding())
            dirToTarget = targetPositionPair.NearestBoundaryNodePosition - transform.position;
        else
            dirToTarget = target.position - transform.position;
        dirToTarget.y = 0;//����� ���� �� �������� �� ��� Y
        Rotate(dirToTarget);

        if (ActionIsAllowed(dirToTarget))
        {
            if (Time.time - _lastAttackTime > _attackInterval)
            {
                UnitAnimation.State = UnitAnimation.States.Attack;
                _lastAttackTime = Time.time;

                StartCoroutine("CauseDamage", target);//���������� ����������� � ���������, ����� ���� �� ���������, ����� �������� ����� ������ ��������
                
                PauseAllTasks();
                State = UnitState.Attack;
                Invoke("StartAllTasks", UnitAnimation.GetCurrentClipLength());
            }
            else
                if (UnitAnimation.State != UnitAnimation.States.Attack)
                {
                    UnitAnimation.State = UnitAnimation.States.IdleBattle;
                }
        }
        else
            UnitAnimation.State = UnitAnimation.States.Run;
    }

    protected virtual IEnumerator CauseDamage(Transform target)
    {
        yield return new WaitForSeconds(_timeBeforeDamageCausing);
        if (!gameObject.IsDied() && target != null) //���� ������ � ���� ��� ����, �� ������� ����
        {
            var hp = target.GetComponent<HP>();
            if (hp != null)
                hp.ChangeHP(-_damage);
        }
        yield break;
    }

}
