using System;
using System.Collections.Generic;
using Pathfinding;
using Shapes;
using UnityEngine;
using System.Collections;
using System.Linq;
using Object = UnityEngine.Object;


public class NodesGrid : RequiredMonoSingleton<NodesGrid>
{
    public class Node
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public Shape Shape { get; private set; }
        public GameObject NotShapeObject;//не Shape
        //public Device device; //под вопросом. 


        public GraphNode AstarNode { get; private set; }
    
        public Vector3 Position 
        { 
            get { return AstarNode.position.ToVector3(); }
        }

        public Node(int x, int y, GraphNode astarNode)
        {
            X = x;
            Y = y;
            AstarNode = astarNode;
        }


        /// <summary>
        /// Проверяет, не занята ли клетка каким-нибудь объектом
        /// </summary>
        public bool IsAvailable
        {
            get { return Shape == null && NotShapeObject == null;/* && device==null*/ }
        }

        /// <summary>
        /// Проверяет, не занята ли клетка каким-нибудь объектом
        /// </summary>
        public void SetShape(Shape shape)
        {
            Shape = shape;
            Shape.Xindex = X;
            Shape.Yindex = Y;
        }

        public void RemoveShape()
        {
            Destroy(Shape.gameObject);
            Shape = null;
        }

        /// <summary>
        /// Очистка ссылок на Shape и NotShapeObject
        /// </summary>
        public void Clear()
        {
            Shape = null;
            NotShapeObject = null;
        } 
    }


    public static Node[,] Grid
    {
        get { return Instance._nodesGrid; }
    }

    //массив направлен снизу вверх.
    private Node[,] _nodesGrid;


    private void Start()
    {
        _nodesGrid = CreateNodesMatrix();
        UpdateNodesData();
    }

    public static Shape GetNextShape(Shape shape, Direction dir)
    {
        if (dir == Direction.Up && shape.Yindex + 1 <= Grid.GetUpperBound(1))
            return Instance._nodesGrid[shape.Xindex, shape.Yindex + 1].Shape;

        if (dir == Direction.Right && shape.Xindex + 1 <= Grid.GetUpperBound(0))
            return Instance._nodesGrid[shape.Xindex + 1, shape.Yindex].Shape;

        if (dir == Direction.Down && shape.Yindex - 1 >= 0)
            return Instance._nodesGrid[shape.Xindex, shape.Yindex - 1].Shape;

        if (dir == Direction.Left && shape.Xindex - 1 >= 0)
            return Instance._nodesGrid[shape.Xindex - 1, shape.Yindex].Shape;

        return null;
    }

    public static List<Shape> FindConnectedNeighborShapes(Shape shape)
    {
        List<Shape> neighbors = new List<Shape>();

        if (shape.Up && shape.Yindex + 1 <= Grid.GetUpperBound(1))
        {
            var neighborShape = Grid[shape.Xindex, shape.Yindex + 1].Shape;
            if (neighborShape != null && neighborShape.Down && !neighborShape.IsInRotateProcess)
                neighbors.Add(neighborShape);
        }

        if (shape.Right && shape.Xindex + 1 <= Grid.GetUpperBound(0))
        {
            var neighborShape = Grid[shape.Xindex + 1, shape.Yindex].Shape;
            if (neighborShape != null && neighborShape.Left && !neighborShape.IsInRotateProcess)
                neighbors.Add(neighborShape);
        }

        if (shape.Down && shape.Yindex - 1 >= 0)
        {
            var neighborShape = Grid[shape.Xindex, shape.Yindex - 1].Shape;
            if (neighborShape != null && neighborShape.Up && !neighborShape.IsInRotateProcess)
                neighbors.Add(neighborShape);
        }

        if (shape.Left && shape.Xindex - 1 >= 0)
        {
            var neighborShape = Grid[shape.Xindex - 1, shape.Yindex].Shape;
            if (neighborShape != null && neighborShape.Right && !neighborShape.IsInRotateProcess)
                neighbors.Add(neighborShape);
        }

        return neighbors;
    }

    /// <summary>
    /// Поиск свободных соседних клеток, которые можно соединить с shape в текущей клетке. 
    /// </summary>
    public static List<KeyValuePair<Node, Direction>> FindAvailableNeighborNodesForShapeSides(Node node)
    {
        var neighbors = new List<KeyValuePair<Node, Direction>>();
        if (node.Shape == null)
            return neighbors;

        var shape = node.Shape;

        if (shape.Up && shape.Yindex + 1 <= Grid.GetUpperBound(1))
        {
            var neighborNode = Grid[shape.Xindex, shape.Yindex + 1];
            neighbors.Add(new KeyValuePair<Node, Direction>(neighborNode, Direction.Down));
        }

        if (shape.Right && shape.Xindex + 1 <= Grid.GetUpperBound(0))
        {
            var neighborNode = Grid[shape.Xindex + 1, shape.Yindex];
            neighbors.Add(new KeyValuePair<Node, Direction>(neighborNode, Direction.Left));
        }

        if (shape.Down && shape.Yindex - 1 >= 0)
        {
            var neighborNode = Grid[shape.Xindex, shape.Yindex - 1];
            neighbors.Add(new KeyValuePair<Node, Direction>(neighborNode, Direction.Up));
        }

        if (shape.Left && shape.Xindex - 1 >= 0)
        {
            var neighborNode = Grid[shape.Xindex - 1, shape.Yindex];
            neighbors.Add(new KeyValuePair<Node, Direction>(neighborNode,Direction.Right));
        }

        return neighbors;
    }


    /// <summary>
    /// Возвращает количество нод, которые пустые или содержат shape, но не содержат другие объекты(мебель, устройства)
    /// </summary>
    /// <returns></returns>
    public static int GetNodesCountWithNotShapeObject()
    {
        int count = 0;
        foreach (var node in Grid)
        {
            if (node.NotShapeObject == null)
                count++;
        }
        return count;
    }


    private static Node[,] CreateNodesMatrix()
    {
        var gridGraph = AstarPath.active.astarData.gridGraph;
        var nodesGrid = new Node[gridGraph.width, gridGraph.depth];
        int ypos = Mathf.RoundToInt(AstarPath.active.astarData.gridGraph.center.y);

        for (int i = 0; i < gridGraph.width; i++)
            for (int j = 0; j < gridGraph.depth; j++)
            {
                var astarNode = AstarPath.active.astarData.gridGraph.GetNearest(new Vector3(i, ypos, j)).node;
                nodesGrid[i, j] = new Node(i, j, astarNode);
            }     
        return nodesGrid;
    }

    public static void UpdateNodesData()
    {
        var gridGraph = AstarPath.active.astarData.gridGraph;
        var nodesGrid = Instance._nodesGrid;
        int ypos = Mathf.RoundToInt(AstarPath.active.astarData.gridGraph.center.y);

        for (int i = 0; i < gridGraph.width; i++)
            for (int j = 0; j < gridGraph.depth; j++)
            {
                var node = nodesGrid[i, j];
                node.Clear();

                var collaiders = Physics.OverlapSphere(new Vector3(i, ypos, j), 0.499f);
                collaiders = collaiders.Where(
                    c => c.CompareTag(Consts.Tags.nodeDevice) ||
                    c.CompareTag(Consts.Tags.nodeFurniture) ||
                    c.CompareTag(Consts.Tags.shape)).ToArray();

                if (collaiders.Length > 0)
                {
                    Shape shape = collaiders.Select(c => c.GetComponent<Shape>()).FirstOrDefault();
                    if (shape != null)
                    {
                        node.SetShape(shape);
                    }
                    else
                    {
                        Device device = collaiders.Select(c => c.GetComponent<Device>()).FirstOrDefault();
                        node.NotShapeObject = device != null ? device.gameObject : collaiders.First().gameObject;
                    }
                }
            }
    }

    /*
    #region Tests methods

    private void TestOuts()
    {
        //int x = 2, y = 2; //Line
        //int x = 2, y = 3; //Corner
        int x = 6, y = 0; //Tee
        var outDir = NodesGrid.Grid[y, x].GetOutDirection(Direction.Up);
        Debug.LogWarning(NodesGrid.Grid[y, x].GetType() + " InDir=Up: outDir=" + outDir, NodesGrid.Grid[y, x]);

        outDir = NodesGrid.Grid[y, x].GetOutDirection(Direction.Right);
        Debug.LogWarning(NodesGrid.Grid[y, x].GetType() + " InDir=Right: outDir=" + outDir, NodesGrid.Grid[y, x]);

        outDir = NodesGrid.Grid[y, x].GetOutDirection(Direction.Down);
        Debug.LogWarning(NodesGrid.Grid[y, x].GetType() + " InDir=Down: outDir=" + outDir, NodesGrid.Grid[y, x]);

        outDir = NodesGrid.Grid[y, x].GetOutDirection(Direction.Left);
        Debug.LogWarning(NodesGrid.Grid[y, x].GetType() + " InDir=Left: outDir=" + outDir, NodesGrid.Grid[y, x]);
    }

    private void TestPath()
    {
        //int x = 2, y = 2; //Line
        int x = 2, y = 3; //Corner
        //int x = 6, y = 0; //Tee
        var path = NodesGrid.Grid[y, x].TestPath(Direction.Up);
        foreach (var pointName in path)
            Debug.LogWarning(NodesGrid.Grid[y, x].GetType() + " InDir=Up: " + pointName, NodesGrid.Grid[y, x]);
        Debug.LogWarning("--------------------------------");

        path = NodesGrid.Grid[y, x].TestPath(Direction.Right);
        foreach (var pointName in path)
            Debug.LogWarning(NodesGrid.Grid[y, x].GetType() + " InDir=Right: " + pointName, NodesGrid.Grid[y, x]);
        Debug.LogWarning("--------------------------------");

        path = NodesGrid.Grid[y, x].TestPath(Direction.Down);
        foreach (var pointName in path)
            Debug.LogWarning(NodesGrid.Grid[y, x].GetType() + " InDir=Down: " + pointName, NodesGrid.Grid[y, x]);
        Debug.LogWarning("--------------------------------");

        path = NodesGrid.Grid[y, x].TestPath(Direction.Left);
        foreach (var pointName in path)
            Debug.LogWarning(NodesGrid.Grid[y, x].GetType() + " InDir=Left: " + pointName, NodesGrid.Grid[y, x]);
        Debug.LogWarning("--------------------------------");
    }

    #endregion
    */
}
