using UnityEngine;
using System.Collections;

public class SceneContainers: RequiredMonoSingleton<SceneContainers>
{
    [SerializeField]
    private Transform _roomContent;

    [SerializeField]
    private Transform _shapes;

    [SerializeField]
    private Transform _connectors;

    [SerializeField]
    private Transform _signals;
    
    public static Transform RoomContent
    {
        get { return Instance._roomContent; }
    }

    public static Transform Shapes
    {
        get { return Instance._shapes; }
    }

    public static Transform Connectors
    {
        get { return Instance._connectors; }
    }

    public static Transform Signals
    {
        get { return Instance._signals; }
    }
}
