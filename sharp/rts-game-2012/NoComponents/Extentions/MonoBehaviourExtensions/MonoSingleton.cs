using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviourHeritor where T : MonoSingleton<T>
{
    protected static T _instance;

    protected static T Current
    {
        get
        {
            if (_instance == null)
                _instance = new GameObject("InstanceOf" + typeof(T).ToString()).AddComponent<T>();
          
            // Problem during the creation, this should not happen
            if (_instance == null)
                Debug.LogError("Problem during the creation of " + typeof(T).ToString());

            return _instance;
        }
    }

    protected virtual void OnApplicationQuit()
    {
        _instance = null;
    }

    protected override void Awake()
    {
        base.Awake();
        if (_instance != null)
            Debug.LogError("Object with " + _instance.GetType() + " script could be single only");
        _instance = this as T;
    }
}
