using UnityEngine;
using System.Collections;

public class ResourcesLoader : MonoSingleton<ResourcesLoader>
{
    private const string _arrowPath = "Prefabs/Bullets/Arrow";
    private const string _skeletonPath = "Prefabs/Mobs/skeleton";
    private const string _bigSkeletonPath = "Prefabs/Mobs/BigSkeleton";
    private const string _skeletonArcherPath = "Prefabs/Mobs/SkeletonArcher";

    private Transform _arrowPrefab;
    private Transform _skeletonPrefab;
    private Transform _bigSkeletonPrefab;
    private Transform _skeletonArcherPrefab;

    public static Transform ArrowPrefab
    {
        get
        {
            if (ReferenceEquals(Instance._arrowPrefab, null))
                Instance._arrowPrefab = LoadPrefab(_arrowPath);
            return Instance._arrowPrefab;
        }
    }

    public static Transform SkeletonPrefab
    {
        get
        {
            if (ReferenceEquals(Instance._skeletonPrefab, null))
                Instance._skeletonPrefab = LoadPrefab(_skeletonPath);
            return Instance._skeletonPrefab;
        }
    }

    public static Transform BigSkeletonPrefab
    {
        get
        {
            if (ReferenceEquals(Instance._bigSkeletonPrefab, null))
                Instance._bigSkeletonPrefab = LoadPrefab(_bigSkeletonPath);
            return Instance._bigSkeletonPrefab;
        }
    }

    public static Transform SkeletonArcherPrefab
    {
        get
        {
            if (ReferenceEquals(Instance._skeletonArcherPrefab, null))
                Instance._skeletonArcherPrefab = LoadPrefab(_skeletonArcherPath);
            return Instance._skeletonArcherPrefab;
        }
    }

    //protected override void Awake()
    //{
    //    base.Awake();
    //}

    private static Transform LoadPrefab(string path)
    {
        var prefab = (Transform)Resources.Load(path);

        if (prefab == null)
        {
            Debug.LogError("Cannot Load Prefab " + path);
            return null;
        }
        return prefab;
    }

}
