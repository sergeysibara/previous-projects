public class MyMecanimSMController : StateMachineBehaviour
{
    public MecanimStateData CurrentState;

    public void OnMyStateEnterCompleted()
    {
        if (CurrentState!= null)
            Debug.Log(CurrentState.Name);
    }
}