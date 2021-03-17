using System.Collections.Generic;
using System.Linq;
using Shapes;
using UnityEngine;
using System.Collections;

public class ConnectorsManager : RequiredMonoSingleton<ConnectorsManager>
{
    public static Connector StartConnector
    {
        get
        {
            if (Instance._startConnector == null)
                Instance.Init();

            return Instance._startConnector;
        }
    }

    /// <summary>
    /// Все коннекторы, которые ожидают прихода сигнала
    /// </summary>
    public static Connector[] TargetConnectors
    {
        get
        {
            if (Instance._targetConnectors == null)
                Instance.Init();
            return Instance._targetConnectors;
        }
    }

    private Connector _startConnector = null;
    private Connector[] _targetConnectors;
    private List<Connector> _unConnectedConnectors = new List<Connector>();

    private List<Shape> _traversedShapes = new List<Shape>();

    private void Start()
    {
    }

    private void Init()
    {
        List<Connector> targetConnectors = new List<Connector>();
        foreach (Transform connector in SceneContainers.Connectors)
        {
            var c = connector.GetComponent<Connector>();
            if (c.IsStartConnector)
            {
                if (_startConnector == null)
                    _startConnector = c;
                else
                    Debug.LogError("Флаг `IsStartConnector=true` больше чем у одного коннектора в сцене");
            }
            else
            {
                targetConnectors.Add(c);
            }
        }

        _targetConnectors = targetConnectors.ToArray();

        if (_startConnector == null)
            Debug.LogError("_startConnector not found");
    }

    /// <summary>
    /// Проверяет, был ли подключен коннектор при последней проверке CheckAllConnections()
    /// </summary>
    public static bool IsConnectedAtLastChecking(Connector c)
    {
        return !Instance._unConnectedConnectors.Contains(c);
    }

    public static bool IsAllConnected()
    {
        CheckAllConnections();
        return Instance._unConnectedConnectors.Count == 0;
    }

    public static int GetConnectedCount()
    {
        CheckAllConnections();
        return TargetConnectors.Length - Instance._unConnectedConnectors.Count;
    }

    public static void CheckAllConnections()
    {
        Instance._unConnectedConnectors.Clear();
        Instance._unConnectedConnectors.AddRange(TargetConnectors);

        //if Has Start Connection
        if (StartConnector.NearestNode.Shape.HasConnection(StartConnector.CurrentDirection))
        {
            Instance.CheckConnectRecursively(StartConnector.NearestNode.Shape);
            Instance._traversedShapes.Clear();
        }
        foreach (var c in Instance._unConnectedConnectors)
            c.SwitchToOff();
    }

    public static Connector FindConnectorWithNearestShape(Shape shape)
    {
        var connector = TargetConnectors;
        return connector.FirstOrDefault(c => c.NearestNode.Shape == shape);
    }

    private void CheckConnectRecursively(Shape shape)
    {
        _traversedShapes.Add(shape);

        var connector = FindConnectorWithConnection(shape);
        if (connector != null && shape.HasConnection(connector.CurrentDirection))
        {
            _unConnectedConnectors.Remove(connector);
        }

        var neighbors = NodesGrid.FindConnectedNeighborShapes(shape);
        foreach (var neighbor in neighbors)
        {
            if (!_traversedShapes.Contains(neighbor))
                CheckConnectRecursively(neighbor);
        }
    }

    private static Connector FindConnectorWithConnection(Shape shape)
    {
        return Instance._unConnectedConnectors.FirstOrDefault(c => c.NearestNode.Shape == shape);
    }
}
