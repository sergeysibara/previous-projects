using UnityEngine;
using System.Collections;

public class TowerAI : BuildingAI, IFarRangeAttacker
{
    #region Inspector Variables

    [SerializeField]
    GameObject _arrowPrefab;

    [SerializeField]
    float _arrowSpeed = 100.0f;

    [SerializeField]
    float _attackDistance = 10.0f;

    [SerializeField]
    int _damage = 7;

    [SerializeField]
    float _attackInterval = 2.5f;

    #endregion


    public LayerMask EnemyLayerMask { get; set; }
    public int BulletLayer { get; set; }
    public float AttackDistance
    {
        get { return _attackDistance; }
        private set { _attackDistance = value; }
    }

    Transform _target;
    Transform _arrowStartPoint;

    const float _searchTargetInterval = 0.5f;
    float _lastAttackTime = -100f; //время, прошедшее с последней атаки. Ставиться отрицательным при инициализации, чтобы атака происходила сразу при нахождении цели
    bool _canSearchTarget = true;//запрет или разрешение поиска цели

    protected override void Awake()
    {
        base.Awake();
        _arrowStartPoint = transform.FindChild("ArrowStartPoint");
        if (_arrowStartPoint == null)
            Debug.LogError("GameObject ArrowStartPoint not found");
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(UpdateTarget());
    }

    void Update()
    {
        _canSearchTarget = true;
        if (TargetIsExist())
        {
            if ((_target.position - transform.position).sqrMagnitude <= _attackDistance * _attackDistance)//(distance <= _attackDistance)
            {
                _canSearchTarget = false;
                Attack();
            }
        }
    }

    public override void Die()
    {
        base.Die();
        _target = null;
        enabled = false;
    }

    IEnumerator UpdateTarget()
    {
        while (true)
        {
            if (_canSearchTarget)
            {
                _target = TargetFindingMethods.FindNearestTarget(transform.position, _attackDistance, EnemyLayerMask);
            }
            yield return new WaitForSeconds(_searchTargetInterval);
        }
    }

    public void Attack(Transform target, TargetPositionPair targetPositionPair)
    {
        Attack();
    }

    void Attack()
    {
        if (Time.time - _lastAttackTime > _attackInterval)
        {
            Bounds targetBounds = _target.collider.bounds;
            Vector3 attackablePoint = targetBounds.center;
            attackablePoint.y += (targetBounds.size.y / 4.0f);

            //создаем и выпускаем стрелу по направлению к врагу
            var forward = attackablePoint - _arrowStartPoint.position;

            Quaternion rotationAttack;
            if (forward != Vector3.zero)
                rotationAttack = Quaternion.LookRotation(forward);
            else
                rotationAttack = Quaternion.Euler(Vector3.zero);
            Vector3 dirAttack = forward.normalized;

            GameObjectManager.CreateArrow(_arrowPrefab, _arrowStartPoint.position, rotationAttack,
                BulletLayer, dirAttack * _arrowSpeed, EnemyLayerMask, _damage);

            _lastAttackTime = Time.time;
        }
    }

    bool TargetIsExist()
    {
        if (_target == null)
        {
            return false;
        }
        else
            if (_target.gameObject.IsDied())
            {
                _target = null;
                return false;
            }
        return true;
    }

}
