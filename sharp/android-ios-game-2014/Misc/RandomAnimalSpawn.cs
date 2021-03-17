using UnityEngine;
using System.Collections;

public class RandomAnimalSpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _prefabs;

	void Awake ()
	{
	    var prefab=RandomUtils.GetRandomItem(_prefabs);
        var animal = (GameObject)Object.Instantiate(prefab, transform.position, new Quaternion(0, 0, 0, 0));
	    animal.transform.parent = transform.parent;
	}

}
