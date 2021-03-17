using UnityEngine;

public abstract class Task : PartlySealedMonoBehaviour
{
    public bool IsActive { get; protected set; }
    public bool IsParallel { get; protected set; }
    //bool IsBlocked(){ get; set;}

    public abstract void SetPerformer<T>(T obj) where T : BaseObjectAI;
    public abstract void SetTaskData<T>(T data) where T : ITaskData;
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

    public virtual void CompleteTask()
    {
        PauseTask();
    }

    protected abstract void ClearTemporaryVariables();

    protected virtual void Awake()
    {
        enabled = false;
    }
}