using UnityEngine;
/// <summary>
/// ������� ����� ��� �������� ����, ������ ������� �� ������ ���� ������ ��������� � transform, collider,rigidbody � �.�.
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
