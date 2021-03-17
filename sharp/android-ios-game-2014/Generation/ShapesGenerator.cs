using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using Shapes;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Node = NodesGrid.Node;

//1. от коннектора создаются провода с тремя соединенями во всех направлениях.
//2. Вращение всех shape так, чтобы не пропали соединения с коннекторами. Также генерирует shape-ы, если у повернутого shape есть свободные выходы в соседние пустые ноды.
//3. Потом идет замены всех проводов на рендомный, с проверкой, чтобы оставалась возможность соединений.
//4. Заполнение оставшихся пустых клеток
//5. Рендомный поворот всех проводов с проверкой, чтобы не был соединен ни один коннектор

public static class ShapesGenerator
{
    private const float _shapeYpos = 10.22f;

    private static int _shapesCount;

    public static void Generate()
    {
        _shapesCount = 0;

        var astarNode = AstarPath.active.astarData.gridGraph.GetNearest(ConnectorsManager.StartConnector.transform.position).node;
        VectorInt2 nodeIndex = astarNode.position.ToVector3();
        var node = NodesGrid.Grid[nodeIndex.x, nodeIndex.y];

        GenerateRecursively(node, ConnectorsManager.StartConnector.CurrentDirection);

        int i = 0;
        bool isAllConnected = false;
        while (i < 100 && isAllConnected == false)
        {
            RandomSafeRotateWithGenerationAllShapes();
            isAllConnected = ConnectorsManager.GetConnectedCount() == ConnectorsManager.TargetConnectors.Count();
            i++;
        }
        //if (isAllConnected)
        //    Debug.LogWarning("<color=green>AllConnected=true</color> iterations_count=" + i);


        RandomReplacementAllShapes();
        ShapesPathBaker.FindAndSavePath();

        FillEmptyNodes();

        RandomRotateAllShapes();
        //проверка, чтобы не был соединен ни один коннектор.
        while (ConnectorsManager.GetConnectedCount() > 0)
        {
            //Debug.LogWarning("<color=cyan>ConnectedCount>0</color>");
            RandomRotateAllShapes();
        }

        //EventMessenger.SendMessage(GameEvent.CompleteGeneration, null); //typeof(ShapesGenerator));
    }


    private static void GenerateRecursively(Node node, Direction prevOutDirection)
    {
        if (!node.IsAvailable)
            return;

        CreateShape(node, typeof (TeeShape));
        FastRotateToConnection((TeeShape)node.Shape, prevOutDirection);

        //генерация ноды, если свободная клетка
        List<KeyValuePair<Node,Direction>> nodes = NodesGrid.FindAvailableNeighborNodesForShapeSides(node);
        foreach (var nodeDirPair in nodes)
        {
            GenerateRecursively(nodeDirPair.Key, nodeDirPair.Value.GetOpposite());
        }
    }


    /// <summary>
    /// Вращение всех shape так, чтобы не пропали соединения с коннекторами. Также генерирует shape-ы, если у повернутого shape есть свободные выходы в соседние пустые ноды.
    /// </summary>
    private static void RandomSafeRotateWithGenerationAllShapes()
    {
        //количество подключенных коннекторов в начале этой функции
        int connectedConnectorsMaxCount = ConnectorsManager.GetConnectedCount();

        foreach (var node in NodesGrid.Grid)
        {
            if (node.Shape != null)
            {
                Direction currentDir = node.Shape.CurrentDirection;
                RandomRotateShape(node.Shape);

                int connectedCount = ConnectorsManager.GetConnectedCount();

                //возврат к состоянию до рэндомного вращения
                if (connectedCount < connectedConnectorsMaxCount)
                    node.Shape.FastRotateToDirection(currentDir);

                connectedConnectorsMaxCount = Mathf.Max(connectedConnectorsMaxCount, connectedCount);
            }

            //генерация ноды, если свободная клетка
            List<KeyValuePair<Node, Direction>> nodes = NodesGrid.FindAvailableNeighborNodesForShapeSides(node);
            foreach (var nodeDirPair in nodes)
            {
                GenerateRecursively(nodeDirPair.Key, nodeDirPair.Value.GetOpposite());
            }
        }
    }

    /// <summary>
    /// Рэндомная замена с проверкой существования пути
    /// </summary>
    private static void RandomReplacementAllShapes()
    {  
        //количество подключенных коннекторов в начале этой функции
        int connectedConnectorsMaxCount = ConnectorsManager.GetConnectedCount();

        foreach (var node in NodesGrid.Grid)
        {
            if (node.Shape != null)
            {
                Direction currentDir = node.Shape.CurrentDirection;
                Type currentShapeType = node.Shape.GetType();

                RemoveShape(node);
                CreateRandomShape(node);

                //вращение с целью попытаться соединить ноду с другими
                int connectedCount = ConnectorsManager.GetConnectedCount();
                int i = 0;
                while (i < 3 && connectedCount < connectedConnectorsMaxCount)
                {
                    node.Shape.FastRotate();
                    connectedCount = ConnectorsManager.GetConnectedCount();
                    i++;
                }

                //возврат к состоянию до рендома
                if (connectedCount < connectedConnectorsMaxCount)
                {
                    RemoveShape(node);
                    CreateShape(node, currentShapeType);
                    node.Shape.FastRotateToDirection(currentDir);
                }

                connectedConnectorsMaxCount = Mathf.Max(connectedConnectorsMaxCount, connectedCount);
            }
        }
    }

    private static void RandomRotateAllShapes()
    {
        foreach (var node in NodesGrid.Grid)
        {
            if (node.Shape != null)
                RandomRotateShape(node.Shape);
        }
    }

    private static void CreateRandomShape(Node node)
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                CreateShape(node, typeof(LineShape));
                break;
            case 1:
                CreateShape(node, typeof(CornerShape));
                break;
            default:
                CreateShape(node, typeof(TeeShape));
                break;
        }
    }

    private static void CreateShape(Node node, Type shapeType)
    {
        GameObject prefab = null;
        switch (shapeType.Name)
        {
            case "LineShape":
                prefab = ResourcesLoader.LineShapePrefab;
                break;
            case "CornerShape":
                prefab = ResourcesLoader.CornerShapePrefab;
                break;
            case "TeeShape":
                prefab = ResourcesLoader.TeeShapePrefab;
                break;
            default:
                Debug.LogError("incorrect type - " + shapeType.Name);
                break;
        }

        Vector3 pos = new Vector3(node.X, _shapeYpos, node.Y);
        var shapeGO = (GameObject)Object.Instantiate(prefab, pos, new Quaternion(0, 0, 0, 0));
        shapeGO.transform.parent = SceneContainers.Shapes;
        
        Shape retShape = shapeGO.GetComponent<Shape>();
        node.SetShape(retShape);
        _shapesCount++;
    }

    private static void RemoveShape(Node node)
    {
        node.RemoveShape();
        _shapesCount--;
    }

    private static void RandomRotateShape(Shape shape)
    {
        Direction dir = (Direction) Random.Range(0, 4);
        shape.FastRotateToDirection(dir);
    }

    /// <summary>
    /// Вращает teeShape, пока он не будет соединен с другим, если это возможно. Если teeShape расположен в ноде с коннектором, то будет требоваться соединение еще и с коннектором
    /// </summary>
    private static void FastRotateToConnection(TeeShape teeShape, Direction direction)
    {
        for (int i = 0; i < 4; i++)
        {
            if (teeShape.HasConnection(direction))
            {
                var connector = ConnectorsManager.FindConnectorWithNearestShape(teeShape);
                if (connector == null)
                    return;
                if (teeShape.HasConnection(connector.CurrentDirection))
                    return;
            }
            if (i < 3)
                teeShape.FastRotate();
        }
        Debug.LogError("connection not found");
    }


    private static void FillEmptyNodes()
    {
        int needShapesCount = NodesGrid.GetNodesCountWithNotShapeObject();
        if (_shapesCount == needShapesCount)
            return;

        //if (shapesCount!=0)
        //    Debug.LogWarning("<color=green>FillEmptyNodes()</color> count=" + shapesCount);

        foreach (var node in NodesGrid.Grid)
        {
            if (node.IsAvailable)
                CreateRandomShape(node);
        }
    }

}
