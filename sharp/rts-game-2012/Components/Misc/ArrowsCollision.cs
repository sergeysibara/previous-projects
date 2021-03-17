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

    bool _collisionAlreadyHasBeen;//на тот случай чтобы не были поражены одновременно 2 цели

    void Start()
    {
        Destroy(gameObject, _destroyTime);
    }

    void OnTriggerEnter(Collider col)
    {
        if (!_collisionAlreadyHasBeen)
        {
            //уничтожаем компоненты, чтобы стрела больше не влияла на другие объекты 
            _collisionAlreadyHasBeen = true;
            rigidbody.isKinematic = true;
            collider.enabled = false;
            Destroy(rigidbody);
            //Destroy(collider);

            if (EnemyLayerMask.IsLayerInLayerMask(col.gameObject.layer))//если попадание по врагу
            {
                var hp = col.GetComponent<HP>();
                if (hp != null)
                    hp.ChangeHP(-Damage);

                Destroy(gameObject);
            }
            else
            {
                if (GameManager.GroundLayers.IsLayerInLayerMask(col.gameObject.layer))
                    transform.parent = col.transform; //застревание стрелы в земле
                else
                    Destroy(gameObject);
            }
        }
    }

}
