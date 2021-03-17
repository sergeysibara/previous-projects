using UnityEngine;

/// <summary>
/// Базовый класс для скриптов игры. (использовать вместо MonoBehaviour)
/// </summary>
public abstract class MonoBehaviourHeritor : MonoBehaviour
{
    private Transform _transform;
    private Collider _collider;
    private Rigidbody _rigidbody;

    public new Transform transform
    {
        get { return _transform; }
    }

    public new Collider collider
    {
        get { return _collider; }
    }

    public new Rigidbody rigidbody
    {
        get { return _rigidbody; }
    }
    

    protected virtual void Awake()
    {
        _transform = base.transform;
        _collider = base.collider;
        _rigidbody = base.rigidbody;
    }
}
