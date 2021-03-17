using UnityEngine;
using System.Collections;

public class ArrowController : MonoBehaviour
{
    [SerializeField]
    private float _speed = 20f;

    private bool _canMove;
    private int _attackPower;
    private Vector3 _normalizedDirection;

    private float _startDistanceToTarget;
    private float _traversedPathLength;

    /// <summary>
    /// 
    /// </summary>    
    public void Shoot(Transform target, int attackPower)
    {
        _canMove = true;
        transform.LookAt(target);
        transform.parent = SceneContainers.Units;//todo bullets
        _normalizedDirection = Getters.Base.GetDirection(transform.position, target.position).normalized;
        _startDistanceToTarget = Getters.Base.GetDistance(transform.position, target.position);
        _attackPower = attackPower;
    }
	
	void Update () 
	{
	    if (_canMove)
	    {
	        var prevPos = transform.position;
            Actions.Position.MoveToDirection(transform, _normalizedDirection, _speed);
	        _traversedPathLength += Getters.Base.GetDistance(prevPos, transform.position);
            //DebugUtils.DrawVerticalRay(transform.position, 10, Color.cyan);
	        if (_traversedPathLength >= _startDistanceToTarget)
            {
                EventAggregator.PublishT(GameEvent.OnPlayerDamage, this, new Damage(DamageType.Far, _attackPower));
                DestroySelf();
            }
	    }
	}

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
