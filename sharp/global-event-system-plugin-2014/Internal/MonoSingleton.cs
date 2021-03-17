using UnityEngine;

namespace EventSystem
{
    /// <summary>
    /// Standart singleton. Create global DontDestroyOnLoad object.
    /// </summary>
	public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        private static bool _isShuttingDown;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (_isShuttingDown)
                        return null;

                    _instance = new GameObject("InstanceOf" + typeof (T)).AddComponent<T>();
                    if (_instance == null)
                        Debug.LogError("Object with " + typeof (T) + " script is not added in this scene");
                }
                return _instance;
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _instance = null;
            _isShuttingDown = true;
        }

        protected virtual void Awake()
        {
            if (_instance != null)
            {
                Debug.LogWarning("Object with " + typeof (T) + " script could be single only", this);
                Destroy(gameObject);
                return;
            }
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }
}