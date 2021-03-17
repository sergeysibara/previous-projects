using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;
using Shapes;
using Random = UnityEngine.Random;

public partial class RoomContentGenerator : RequiredMonoSingleton<RoomContentGenerator>
{
    [SerializeField, Range(2,3)]
    private int _nodesBetweenCorners = 2;

    [SerializeField]
    private Transform _sofaPrefab;

    [SerializeField]
    private Transform _plasmaTVPrefab;

    [SerializeField]
    private Transform _connectorPrefab;

    [SerializeField]
    private Transform _phonePrefab;

    [SerializeField]
    private Transform[] _windowPrefabs;

    [SerializeField]
    private Transform[] _coversPrefabs;
    
    [SerializeField]
    private Transform[] _chairPrefabs;

    [SerializeField]
    private Transform[] _compPrefabs;


    private Transform _furniture;
    private Transform _devices;
    private Transform _windows;
    //private Transform _cat;
    private List<SpawnNode> _allNodes = new List<SpawnNode>();
    private List<SpawnNode> _emptyNodes = new List<SpawnNode>();

    private const float localOffset = 3f;

    protected override void Awake()
    {
        base.Awake();
        _furniture = SceneContainers.RoomContent.Find("Furniture");
        _devices = SceneContainers.RoomContent.Find("Devices");
        _windows = SceneContainers.RoomContent.Find("Windows");
        //_cat = GameObject.Find("Dynamic/Cat").transform;

        if (Instance._sofaPrefab == null)
            Debug.LogException(new Exception("_sofaPrefab is null"), Instance);
        if (Instance._plasmaTVPrefab == null)
            Debug.LogException(new Exception("_plasmaTVPrefab is null"), Instance);
    }


    public static void Generate()
    {
        Instance.FillNodes();

        Direction sofaDir;
        int xIndex, yIndex;
        Vector3 sofa_pos;
        Instance.CreateSofa(out sofa_pos, out sofaDir, out xIndex, out yIndex);
        Instance.CreateWindows(sofaDir);

        Vector3 plasmaPos;
        Instance.CreatePlasma(sofa_pos, sofaDir, out plasmaPos);
        Instance.CreatePlasmaConnector(plasmaPos, sofaDir);

        Instance.CreateStartConnector(sofaDir);

        Instance.CreateComp();
        Instance.CreatePhone();


        int chairCount = Random.Range(1, 3);
        int coverCount = Random.Range(2, 4);
        var chairPrefab = RandomUtils.GetRandomItem(Instance._chairPrefabs);
        var coverPrefab = RandomUtils.GetRandomItem(Instance._coversPrefabs);

        Instance.CreateCovers(chairCount, chairPrefab);
        Instance.CreateCovers(coverCount, coverPrefab);

        AstarPath.active.Scan();
        NodesGrid.UpdateNodesData();
        Instance.DebugEmptyNodes();
    }
   
    private static Vector3 RoomClamp(Vector3 point)
    {
        const float min = -3.5f + localOffset;
        const float max = 3.5f + localOffset;
        point.x = Mathf.Clamp(point.x, min, max);
        point.z = Mathf.Clamp(point.z, min, max);
        return point;
    }

}
