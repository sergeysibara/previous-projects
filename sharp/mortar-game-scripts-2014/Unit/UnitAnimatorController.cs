using UnityEngine;
using System.Collections;
using Pathfinding.RVO;

//todo класс params - как в az добавить
public class UnitAnimatorController : MonoBehaviour
{
    [SerializeField, TooltipAttribute("скорость изменения параметра анимации 'Speed' в секунду")]
    private float _changingSpeed = 2;

    [SerializeField, ReadOnlyInInspector]
    private float _speedInfo;

    private Vector3 _prevPos;
    private float _prevSpeed;
    private Animator _animator;
    private RVOController _rvo;

    private int _prevState;
    //private int _currentState;

	void Start ()
	{
	    _animator = GetComponent<Animator>();
        _prevState = _animator.GetCurrentAnimatorStateInfo(0).nameHash;

        _rvo = GetComponent<RVOController>();
	    _prevPos = transform.position;
	}

    private void Update()
    {
        UpdateSpeed();
        UpdateCurrentState();
    }


    void UpdateSpeed()
	{
        float dist  = Vector3.Distance(transform.position, _prevPos);
        _speedInfo = Mathf.Lerp(0, _rvo.maxSpeed, dist / (_rvo.maxSpeed * Time.deltaTime));//currentvalue/maxvalue

        _speedInfo = MathfUtils.SmoothChangeValue(_prevSpeed, _speedInfo, _changingSpeed, Time.deltaTime, 0, _rvo.maxSpeed);
	    //_speedInfo = Mathf.SmoothDamp(_prevSpeed, _speedInfo, ref cv, 0.5f, 1, Time.deltaTime);

	    _animator.SetFloat("Speed", _speedInfo);
	    _prevPos = transform.position;
	    _prevSpeed = _speedInfo;
	}

    public void StartAttack()
    {
         _animator.SetTrigger("StartAttackMode");        
    }
    public void StoptAttack()
    {
        _animator.SetTrigger("StopAttackMode");
    }


    /// <summary>
    /// Обновления информации о текущем стейте и рассылка уведомления о смене состояния
    /// </summary>
    private void UpdateCurrentState()
    {
        int currState = _animator.GetCurrentAnimatorStateInfo(0).nameHash;
        //if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Base.move"))
        //    Debug.LogWarning("Base.move");
        //if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Base.attack"))
        //    Debug.LogWarning("Base.attack");
        if (_prevState != currState)
            OnChangeState();

        _prevState = currState;
        //todo потом еще желательно уведомлять о смене анимаций Move и Idle
    }

    private void OnChangeState()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Base.move"))
        {
            //Debug.LogWarning("SendMessage");
            SendMessage("Shoot");//todo потом сделать отдельно (скорее в аниматоре сделать такие состояния: Move->PreAttack->Shoot->PostAttack->Move ), чтобы отследить событие
            SendMessage("OnAttackEnd");
        }
    

    //Debug.LogWarning(_animator.GetCurrentAnimatorStateInfo(0).IsName("Base.move") + " move");
        //Debug.LogWarning(_animator.GetCurrentAnimatorStateInfo(0).IsName("Base.attack") + " attack");
    }
}