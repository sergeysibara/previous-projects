using UnityEngine;

/// <summary>
/// Local singleton for concrete scene.  Required manually adding to scene.
/// </summary>
public abstract class LocalMonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static bool _isDestroyed;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                if (_isDestroyed)
                {
                    Debug.LogWarning("Object with " + typeof (T) + " script is was destroyed");
                }
                else
                    Debug.LogError("Object with " + typeof (T) + " script is not added in this scene");
            }
            return _instance;
        }
    }

    protected virtual void OnApplicationQuit()
    {
        _isDestroyed = true;
        _instance = null;
    }
    protected virtual void OnDestroy()
    {
        _isDestroyed = true;
        _instance = null;
    }

    protected virtual void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("Object with " + typeof (T) + " script could be single only", this);
            Destroy(gameObject);
            return;
        }
        _isDestroyed = false;
        _instance = this as T;
    }
}
