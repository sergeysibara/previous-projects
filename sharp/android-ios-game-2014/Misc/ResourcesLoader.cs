using UnityEngine;
using System.Collections;

public class ResourcesLoader : MonoSingleton<ResourcesLoader>
{
    private const string _signalPrefabPath = "Prefabs/Signals/OscillographSpark";
    private const string _lineShapePrefabPath = "Prefabs/Shapes/LineShape";
    private const string _cornerShapePrefabPath = "Prefabs/Shapes/CornerShape";
    private const string _teeShapePrefabPath = "Prefabs/Shapes/TeeShape";

    private GameObject _signalPrefab = null;
    private GameObject _lineShapePrefab;
    private GameObject _cornerShapePrefab;
    private GameObject _teeShapePrefab;

    public static GameObject SignalPrefab
    {
        get { return Instance._signalPrefab; }
    }

    public static GameObject LineShapePrefab
    {
        get { return Instance._lineShapePrefab; }
    }

    public static GameObject CornerShapePrefab
    {
        get { return Instance._cornerShapePrefab; }
    }

    public static GameObject TeeShapePrefab
    {
        get { return Instance._teeShapePrefab; }
    }

    protected override void Awake()
    {
        base.Awake();
        _lineShapePrefab = LoadPrefab(_lineShapePrefabPath);
        _cornerShapePrefab = LoadPrefab(_cornerShapePrefabPath);
        _teeShapePrefab = LoadPrefab(_teeShapePrefabPath);
    }

    private GameObject LoadPrefab(string path)
    {
        var prefab = (GameObject)Resources.Load(path);

        if (prefab == null)
        {
            Debug.LogError("Cannot Load Prefab " + path);
            return null;
        }
        return prefab;
    }

}
