using UnityEngine;
using System.Collections;

/// <summary>
/// потом вместо шара должен быть эффект небольшой тени и затем взрыв
/// </summary>
public class CannonballController : MonoBehaviour
{
    [SerializeField]
    private bool _hideInHierarchyInRuntime;

    [SerializeField]
    private float _flyDuration = 1;

    [SerializeField]
    private float _destroyTimeAfterExplosion = 2f;

    private float _speed;
    private float _targetYpos;

    public AudioClip _clip;
    public Transform _explosionPrefab;

    private void Awake()
    {
        //if (_hideInHierarchyInRuntime)
        //    gameObject.hideFlags = HideFlags.HideInHierarchy;
    }

    void Start () 
	{
        RaycastHit hit;
        Ray ray = new Ray(transform.position, -Vector3.up);
        Physics.Raycast(ray, out hit, Mathf.Infinity, Consts.LayerMasks.BallCollisions);
	    
        _speed = hit.distance/_flyDuration;
	    _targetYpos = hit.point.y;

        StartCoroutine(MoveCoroutine());
	}

    private IEnumerator MoveCoroutine()
    {
        //float startTime = Time.time;
        while (transform.position.y > _targetYpos)
        {
            transform.Translate(-Vector3.up * _speed*Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        transform.position = PhysicsUtils.RaycastFromUpToDown(transform.position, Consts.LayerMasks.BallCollisions).point;
        Explosion();
    }

    private float ExplosionSize {
        get { return PlayerStats.Instance.CurrentExplosionSize - 0.1f * PlayerStats.Instance.CurrentExplosionSize; }//0.3f*PlayerStats.Instance.CurrentExplosionSize - для фикс погрешности визуальной части проектора
    }


    /// <summary>
    /// Взрыв
    /// </summary>
    private void Explosion()
    {
        var units = Physics.OverlapSphere(transform.position, ExplosionSize, Consts.LayerMasks.Units);//.OverlapSphere<BaseUnitAI>(transform.position, PlayerStats.Instance.CurrentExplosionSize);
        BonusManager.Instance.CalculateBonusesForExplosion(units, transform.position);
        //Debug.Break();
        foreach (var unit in units)
        {
            //-todo потом вычислять урон линейно в зависимости от дистанции до взрыва, но минимальный урон = 1
            //EventAggregator.OnUnitDamage.PublishToConcrete(PlayerStats.Instance.CurrentExplosionPower, this, unit);
            unit.SendMessage(Consts.Events.Damage, PlayerStats.Instance.CurrentExplosionPower);//, SendMessageOptions.DontRequireReceiver);            
        }

        //overlapSphere + sendMessage

        //временно тут бонус иконку отображать
        //BonusManager.Instance.AttachBonus(transform.position);
        Invoke("DestroySelf",_destroyTimeAfterExplosion);

        renderer.enabled = false;
        AudioSource.PlayClipAtPoint(_clip, transform.position,1f);

        Transform fx = (Transform)Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        fx.parent = transform;

        ParticleEmitter pe = fx.GetComponent<ParticleEmitter>();
        //Debug.LogWarning(pe.minSize);
        pe.minSize = PlayerStats.Instance.CurrentExplosionSize + 1;
        pe.maxSize = PlayerStats.Instance.CurrentExplosionSize + 1;
        //Debug.LogWarning(pe.minSize);
    }
    
    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue.WithAlpha(0.5f);
        Gizmos.DrawSphere(transform.position, ExplosionSize);
    }


}
