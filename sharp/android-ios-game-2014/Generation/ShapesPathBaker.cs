using System;
using System.Collections.Generic;
using System.Linq;
using Shapes;

public static class ShapesPathBaker
{
    private static readonly List<Shape> _traversedShapes = new List<Shape>();
    private static readonly List<Connector> _unTraversedConnectors = new List<Connector>();

    private static readonly List<ChainItem> _chainItems = new List<ChainItem>(); //для сохранения обхода

    public static void FindAndSavePath()
    {
        _traversedShapes.Clear();
        _chainItems.Clear();
        
        _unTraversedConnectors.AddRange(ConnectorsManager.TargetConnectors);
        TraverseRecursively(ConnectorsManager.StartConnector.NearestNode.Shape, _chainItems);

        ShapesSorter.SetChain(_chainItems);
    }

    private static bool TraverseRecursively(Shape shape, List<ChainItem> chain)
    {        
        _traversedShapes.Add(shape);
        bool chainItemHasConnector = false;

        ChainItem chainItem = new ChainItem();
        chainItem.Shape = shape;
        chainItem.TargetDirection = shape.CurrentDirection;
        chain.Add(chainItem);

        var connector = FindConnectorWithConnection(shape);
        if (connector != null && shape.HasConnection(connector.CurrentDirection))
        {
            //Debug.LogWarning(connector.name);
            chainItemHasConnector = true;
            _unTraversedConnectors.Remove(connector);
        }

        var neighbors = NodesGrid.FindConnectedNeighborShapes(shape);

        bool hasFirstChain = false;
        foreach (var neighbor in neighbors)
        {            
            if (!_traversedShapes.Contains(neighbor))
            {
                List<ChainItem> targetChain = chain;
                if (hasFirstChain)
                    targetChain = chainItem.childChain;

                bool nextChainItemHasConnector = TraverseRecursively(neighbor, targetChain);
                chainItemHasConnector |= nextChainItemHasConnector;
                hasFirstChain = true;
            }
        }

        //если текущая нода не соединена с коннектором и дочерние ответвления тоже не соединены с коннектором, то удалить данный узел.
        if (!chainItemHasConnector)
        {
            //chainItem.Shape.name += "_RemovedChainItem";
            chain.Remove(chainItem);
        }

        return chainItemHasConnector;
    }

    private static Connector FindConnectorWithConnection(Shape shape)
    {
        return _unTraversedConnectors.FirstOrDefault(c => c.NearestNode.Shape == shape);
    }

}
