using System;
using UnityEngine;
using System.Collections;

//экземпляры данного класса располагать на сцене, чтобы не инстансить их постоянно. В бонус manager хранить ссылки на данные экземпляры.
 [Serializable]
public abstract class BaseBonus : MonoBehaviour
{
    public string SpriteName;

    /// <summary>
    /// Краткое описание бонуса.  
    /// </summary>
    public string LocalizationKey;//todo должно бвть локализировано

    /// <summary>
    /// Постоянный бонус, действует в течении всего уровня
    /// </summary>    
    public virtual bool IsConstant { get; private set; }

    /// <summary>
    /// Выполняется ли бонус в данный момент или нет.
    /// </summary>
    public bool IsActive { get; protected set; }


    /// <summary>
    /// Длительность, для не Constant бонусов
    /// </summary>
    public float Duration;
    
    protected float _remainingTime;

    public float RemainingTime
    {
        get { return _remainingTime; }
    }

    public virtual void RunEffect()
    {
        if (IsActive)
            Debug.LogError("bonus already runned", this);
        IsActive = true;
        _remainingTime = Duration;
        StartCoroutine(RemainingTimeCounterCoroutine());
        EventAggregator.PublishT(GameEvent.OnStartBonusEffect, this, this);
    }
    public virtual void ResetDuration()
    {
        _remainingTime = Duration;
    }

    protected virtual void DeleteEffect()
    {
        IsActive = false;
        EventAggregator.PublishT(GameEvent.OnEndBonusEffect, this, this);
    }

    protected IEnumerator RemainingTimeCounterCoroutine()
    {
        while (_remainingTime>0)
        {
            _remainingTime -= Time.deltaTime;
            yield return null;
        }
        DeleteEffect();
    }

     protected void Test()
     {
         //Duration = 3;
         RunEffect();
     }
}
