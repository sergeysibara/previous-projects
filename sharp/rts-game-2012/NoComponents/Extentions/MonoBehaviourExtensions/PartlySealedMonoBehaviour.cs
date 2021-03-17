using UnityEngine;
/// <summary>
/// Базовый класс для скриптов игры, внутри которых не должно быть прямых обращений к transform, collider,rigidbody и т.п.
/// </summary>
public abstract class PartlySealedMonoBehaviour : MonoBehaviour
{
    private new Transform transform
    {
        get { return null; }
    }

    private new Collider collider
    {
        get {  return null; }
    }

    private new Rigidbody rigidbody
    {
        get {  return null; }
    }

}
