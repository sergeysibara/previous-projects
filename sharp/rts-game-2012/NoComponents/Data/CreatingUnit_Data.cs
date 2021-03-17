using UnityEngine;
using System.Collections;

[System.Serializable]
public class CreatingUnit_Data
{
    public GameObject Prefab
    {
        get { return _prefab; }
    }

    public int Price
    {
        get { return _price; }
    }

    public float CreatingDuration
    {
        get { return _creatingDuration; }
    }

    GameObject _prefab;
    int _price;
    float _creatingDuration;

    public CreatingUnit_Data(GameObject prefab, int price, float creatingDuration)
    {
        if (!prefab.IsUnit())
            Debug.LogError("Prefab has not " + Tags.Unit + " tag");

        _prefab = prefab;
        _price = price;
        _creatingDuration = creatingDuration;
    }
}
