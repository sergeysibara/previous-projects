from http://unity3d.ru/distribution/viewtopic.php?f=105&t=31486
/**
* unsing:
* var fsm=GetComponent<Animator>().GetBehaviour<MyMecanimSMController >();
* var stateName = fsm.CurrentState.name;
*/

public class MecanimStateData : StateMachineBehaviour
{
    public string Tag;
    public string Name;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var fsm = animator.GetBehaviour<MyMecanimSMController>();
        fsm.CurrentStateData = this;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var fms = animator.GetBehaviour<MyMecanimSMController>();
        fms.OnMyStateEnterCompleted();
    }
}