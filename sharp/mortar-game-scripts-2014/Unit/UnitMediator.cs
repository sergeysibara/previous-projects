using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Pathfinding;
using Pathfinding.RVO;
public class UnitMediator : MonoBehaviour 
{
    public MyRichAI RichAI { get; private set; }
    public RVOController RVOController { get; private set; }
    public UnitStats Stats { get; private set; }
    public UnitAnimatorController AnimatorController { get; private set; }
    public BaseUnitBehaviour CurrentBehaviourState { get; private set; }

    public ArcherFSMVariables FsmVariables { get; private set; }

    private Dictionary<UnitState, BaseUnitBehaviour> _states = new Dictionary<UnitState, BaseUnitBehaviour>();
	void Awake () 
	{
        RichAI = GetComponent<MyRichAI>();
        RVOController = GetComponent<RVOController>();
        Stats = GetComponent<UnitStats>();
        AnimatorController = GetComponent<UnitAnimatorController>();

        var ai = GetComponent<WarriorBehaviour_Normal>();//Warrior_NormalState
	    if (ai!=null)
	    {
            _states.Add(UnitState.Normal, GetComponent<WarriorBehaviour_Normal>());
            _states.Add(UnitState.Retreat, GetComponent<UnitBehaviour_Retreat>());
	    }
	    else //если юнит - лучник
	    {
            _states.Add(UnitState.Normal, GetComponent<ArcherBehaviour_Normal>());
            _states.Add(UnitState.Attack, GetComponent<ArcherBehaviour_Attack>());
            _states.Add(UnitState.Retreat, GetComponent<UnitBehaviour_Retreat>());
            FsmVariables = GetComponent<ArcherFSMVariables>();
	    }

	    CurrentBehaviourState = _states[UnitState.Normal];



        if (!EventAggregator.IsApplicationShuttingDown)
        {
            //Debug.LogWarning(gameObject.GetInstanceID());
            EventAggregator.PublishT(GameEvent.OnAddUnitToGame, this, Stats.SpawnRate);
        }

        //RichAI.repathRate = Random.Range(0.7f, 1.0f);
        //Debug.LogWarning(RichAI.repathRate);
	}

    void Start()
    {
        EventAggregator.Subscribe(GameEvent.EngGameProcess, this, SetRetreatState);

    }

    public void SetState(UnitState state)
    {
        if (CurrentBehaviourState==null)
            return;

        CurrentBehaviourState.enabled = false;
        CurrentBehaviourState =_states[state];
        CurrentBehaviourState.enabled = true;
    }

    void Update () 
	{
	    
	}

    private void SetRetreatState()
    {
        //Debug.LogWarning("RetreatState");
        SetState(UnitState.Retreat);
    }


}
