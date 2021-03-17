using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Rigidbody))]
public class ArrowsCollision : MonoBehaviourHeritor 
{
    [SerializeField]
    float _destroyTime = 5;

    [HideInInspector]
    public int Damage { get; set; }

    [HideInInspector]
    public LayerMask EnemyLayerMask { get; set; }

    bool _collisionAlreadyHasBeen;//�� ��� ������ ����� �� ���� �������� ������������ 2 ����

    void Start()
    {
        Destroy(gameObject, _destroyTime);
    }

    void OnTriggerEnter(Collider col)
    {
        if (!_collisionAlreadyHasBeen)
        {
            //���������� ����������, ����� ������ ������ �� ������ �� ������ ������� 
            _collisionAlreadyHasBeen = true;
            rigidbody.isKinematic = true;
            collider.enabled = false;
            Destroy(rigidbody);
            //Destroy(collider);

            if (EnemyLayerMask.IsLayerInLayerMask(col.gameObject.layer))//���� ��������� �� �����
            {
                var hp = col.GetComponent<HP>();
                if (hp != null)
                    hp.ChangeHP(-Damage);

                Destroy(gameObject);
            }
            else
            {
                if (GameManager.GroundLayers.IsLayerInLayerMask(col.gameObject.layer))
                    transform.parent = col.transform; //����������� ������ � �����
                else
                    Destroy(gameObject);
            }
        }
    }

}
