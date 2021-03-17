using UnityEngine;

public abstract class DefaultTask : PartlySealedMonoBehaviour
{
    public bool IsActive { get; protected set; }
    //bool IsBlocked(){ get; set;}

    public abstract void SetPerformer<T>(T obj) where T : BaseObjectAI;
    public abstract void StartTask();

    public virtual void PauseTask()
    {
        if (IsActive)
        {
            StopAllCoroutines();
            IsActive = false;
            enabled = false;
        }
    }

    protected abstract void ClearTemporaryVariables();
    protected abstract void StartParallelTask();
    protected abstract void PauseParallelTask();

    protected virtual void Awake()
    {
        enabled = false;
    }
}
