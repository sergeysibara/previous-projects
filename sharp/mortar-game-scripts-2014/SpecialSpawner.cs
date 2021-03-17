using UnityEngine;
using System.Collections;

public class SpecialSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _prefab;

    //[SerializeField] //для выбора родителя для слзданного объекта 
    //private Transform _parent;

	void Awake ()
	{
	    Instantiate(_prefab, transform.position, transform.rotation);
        Destroy(gameObject);
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow.WithAlpha(0.4f);
        Gizmos.DrawSphere(transform.position,0.5f);

        GizmosUtils.DrawText(CustomEditorPrefs.GizmoGuiSkin, (_prefab != null) ? _prefab.name : "prefab=null", transform.position, Color.blue);
    }

}
