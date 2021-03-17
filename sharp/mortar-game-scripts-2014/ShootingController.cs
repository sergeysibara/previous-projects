using UnityEngine;
using System.Collections;

public class ShootingController : MonoBehaviour
{
    [SerializeField, TooltipAttribute("Префаб ядра пушки")]
    private Transform _cannonballPrefab;

    [SerializeField, Tooltip("Величина прибавки к стартовой позиции выстрела, относительно текущей высоты ShootingController")]
    private float _startShootHeightUnder = 20f;

    [SerializeField]
    private float _projectorSizeAfterShoot=0.5f;

    private Projector _projector; 
    private Color _startColor;

    private bool _isCooldownInProgress;

	void Start () 
	{
	    _projector = GetComponent<Projector>();
	    _startColor = _projector.material.color;
        _projector.orthoGraphicSize = PlayerStats.Instance.CurrentExplosionSize;

	    Material m = new Material(_projector.material);
	    _projector.material = m;

	    EventAggregator.Subscribe(GameEvent.EngGameProcess, this, () => { _projector.material.color = Color.white; });
	}

    /// <summary>
    /// Счетчик времени между touchBegin и touchEnd.
    /// </summary>
    private float _tapDuration=10f;

    private void Update()
    {
        if (BattleManager.Instance.Pause)
            _tapDuration = 10;

        if (BattleManager.CurrentGameMode != GameMode.Normal || BattleManager.Instance.Pause)
            return;

        _tapDuration += Time.deltaTime;

        if (_projector.orthographicSize != PlayerStats.Instance.CurrentExplosionSize && !_isCooldownInProgress)
            _projector.orthographicSize = PlayerStats.Instance.CurrentExplosionSize;

        if (_isCooldownInProgress)
            return;


        if (Application.platform.In(RuntimePlatform.Android, RuntimePlatform.IPhonePlayer))
        {
            for (var i = 0; i < Input.touchCount; ++i)
            {
                //if (Input.GetTouch(i).phase == TouchPhase.Began)
                //{
                //    if (Input.GetTouch(i).tapCount >= 2) // double tap
                //    //if (Input.GetTouch(i).phase.Equals(TouchPhase.)) 
                //    {
                //        Shoot();
                //        return;
                //    }
                //}

                if (Input.GetTouch(i).phase == TouchPhase.Moved) //если было большое перемещение, то сброс таймера
                {
                    //Debug.LogWarning(Input.GetTouch(i).deltaPosition);
                    if (Input.GetTouch(i).deltaPosition.magnitude>10)
                        _tapDuration = 10;
                }
            }
            if (Input.GetMouseButtonDown(0))
                _tapDuration = 0;
            if (Input.GetMouseButtonUp(0))
                TryShoot();

        }
        else if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void TryShoot()
    {
        if (_tapDuration<0.5f)
            Shoot();

        _tapDuration = 0;
    }

    private void Shoot()
    {
        var uicam = BattleManager.Instance.UICamera;
        if (Conditions.GUI.ClickOverGUI(uicam.camera,Consts.LayerMasks.UI))
            return;
        //Debug.LogWarning("shoot");
        Vector3 pos = transform.position + new Vector3(0, _startShootHeightUnder, 0);
        Instantiate(_cannonballPrefab, pos, Quaternion.identity);
        StartCoroutine(ProjectorSizeCoroutine());
    }


    private IEnumerator ProjectorSizeCoroutine()
    {
        _isCooldownInProgress = true;
        _projector.orthographicSize = _projectorSizeAfterShoot;
        _projector.material.color = Color.white;
        float coolDownStartTime = Time.time;

        var cooldownDurationAtStart = PlayerStats.Instance.CurrentShootCooldown;
        float offset = 0f;
        while (!Mathf.Approximately(_projector.orthographicSize, PlayerStats.Instance.CurrentExplosionSize))
        {
            var elapsedTime = Time.time - coolDownStartTime;//прошедшее время со старта кулдауна

            //корректировка прошедщего времени с учетом изменения кулдауна бонусами во время его отчета. 
            if (!Mathf.Approximately(cooldownDurationAtStart,PlayerStats.Instance.CurrentShootCooldown))
            {
                offset = elapsedTime * (PlayerStats.Instance.CurrentShootCooldown / cooldownDurationAtStart) - elapsedTime;
                cooldownDurationAtStart = PlayerStats.Instance.CurrentShootCooldown;
                //Debug.LogWarning("offset="+offset);
                //Debug.LogWarning("elapsedTime=" + elapsedTime);
            }
            elapsedTime += offset;

            _projector.orthographicSize = Mathf.Lerp(_projectorSizeAfterShoot, PlayerStats.Instance.CurrentExplosionSize, elapsedTime / cooldownDurationAtStart);
            //Debug.LogWarning("_projector.orthographicSize=" + _projector.orthographicSize);
            //Debug.LogWarning("PlayerStats.Instance.CurrentExplosionSize=" + PlayerStats.Instance.CurrentExplosionSize);
            //Debug.LogWarning("elapsedTime / cooldownDurationAtStart=" + (elapsedTime / cooldownDurationAtStart));
            yield return null;
        }
        //Debug.LogWarning("_projector.orthographicSize=" + _projector.orthographicSize);
        //Debug.LogWarning("elapsedTime on end=" + (Time.time-coolDownStartTime));
        _projector.orthographicSize = PlayerStats.Instance.CurrentExplosionSize;
        if (BattleManager.CurrentGameMode!= GameMode.Victory)
            _projector.material.color = _startColor;
        _isCooldownInProgress = false;
    }

}
