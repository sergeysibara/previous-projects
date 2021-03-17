using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;
using Shapes;
using Random = UnityEngine.Random;

public partial class RoomContentGenerator
{
    private enum SpawnNodeType
    {
        Empty,
        Connector,
        CornerConnector,
        Device,
        Cover,
        Busy
        //если будут нужны ноды с двумя типами, то просто создать типы анагочные этому - DeviceAndConnector
    }

    private class SpawnNode
    {
        /// <summary>
        /// Индекс в массиве _allNodes
        /// </summary>
        public int Index { get; private set; }

        public SpawnNodeType NodeType = SpawnNodeType.Empty;
    
        public NodesGrid.Node GridNode;

        /// <summary>
        /// Направление, противоположное стене. Для угловых нод, всегда за данное направление отвечает NodesGrid.Node.Y
        /// </summary>
        public Direction MainDirection = Direction.None;

        /// <summary>
        /// Дополнительное направление, противоположное второй стене.(только для угловых нод) 
        /// </summary>
        public Direction DirectionInCover = Direction.None;

        public SpawnNode(NodesGrid.Node gridNode, int index)
        {
            Index = index;
            GridNode = gridNode;

            if (gridNode.Y == 0)
                MainDirection = Direction.Up;
            else if (gridNode.Y == 6)
                MainDirection = Direction.Down;
            else if (gridNode.X == 0)
                MainDirection = Direction.Right;
            else
                MainDirection = Direction.Left;

            if (gridNode.Y == 0 || gridNode.Y == 6)
            {
                if (gridNode.X == 0)
                    DirectionInCover = Direction.Right;
                else if (gridNode.X == 6)
                    DirectionInCover = Direction.Left;
            }
        }
    }

    /// <summary>
    /// Наиболее удаленная свободная нода от types
    /// </summary>
    private SpawnNode GetFarEmptyNodeFrom(params SpawnNodeType[] types)
    {
        Debug.LogWarning("generation: call GetFarEmptyNodeFrom() function");
        //индексы, нод c types
        var indexes = _allNodes.Where(n => n.NodeType.In(types)).Select(n => n.Index).ToList();

        int index = _allNodes.First(n => n.NodeType.In(types)).Index;
        indexes.Add(_allNodes.Count + index);

        //поиск индекса ноды, наиболее отдаленной от 2-х нод с коннекторами
        int maxDist = 0;
        int farIndex = 0;
        for (int i = 1; i < indexes.Count; i++)
        {
            if (indexes[i] - indexes[i - 1] > maxDist)
            {
                maxDist = indexes[i] - indexes[i - 1];
                farIndex = indexes[i - 1] + maxDist / 2;
            }
        }

        //var pos = _allNodes[farIndex].GridNode.Position;
        //Debug.DrawLine(pos, pos + Vector3.up * 4, Color.green, 100f);

        if (_allNodes.Count <= farIndex)
        {
            Debug.LogWarning("farIndex=" + farIndex);
            farIndex -= (_allNodes.Count);
        }

        if (_allNodes[farIndex].NodeType != SpawnNodeType.Empty)
            return GetNearNode(_allNodes[farIndex], SpawnNodeType.Empty);
        
        return _allNodes[farIndex];
    }

    private SpawnNode GetNearNode(SpawnNode node, params SpawnNodeType[] types)
    {
        var nextItems = ListUtils.CreateListFrom(node.Index, _allNodes);
        var prevItems = ListUtils.CreateReversedListFrom(node.Index, _allNodes);

        for (int i = 0; i < nextItems.Count; i++)
        {
            if (nextItems[i].NodeType.In(types))
                return nextItems[i];
            if (prevItems[i].NodeType.In(types))
                return prevItems[i];
        }
        Debug.LogError("Empty node not found");
        return null;
    }

    private int GetNearNodeDistance(SpawnNode node, params SpawnNodeType[] types)
    {
        int distance = 0;
        var nextItems = ListUtils.CreateListFrom(node.Index, _allNodes);
        var prevItems = ListUtils.CreateReversedListFrom(node.Index, _allNodes);

        for (int i = 0; i < nextItems.Count; i++)
        {
            distance++;
            if (nextItems[i].NodeType.In(types) || prevItems[i].NodeType.In(types))
                return distance;
        }
        return distance;
    }


    /// <summary>
    /// Получает самую дальнюю от коннекторов ноду, являющуюся соседней текущей ноде.
    /// </summary>
    private SpawnNode GetFarthestFromConnectorstNeighborNode(SpawnNode node)
    {
        var nextItems = ListUtils.CreateListFrom(node.Index, _allNodes);
        var prevItems = ListUtils.CreateReversedListFrom(node.Index, _allNodes);

        //если соседняя нода занята, то вернуть ноду с другой стороны
        if (nextItems[0].NodeType!= SpawnNodeType.Empty)
            return prevItems[0];
        if (prevItems[0].NodeType != SpawnNodeType.Empty)
            return nextItems[0];

        for (int i = 0; i < nextItems.Count; i++)
        {
            if (nextItems[i].NodeType != SpawnNodeType.Connector)
                return prevItems[0];
            if (prevItems[i].NodeType != SpawnNodeType.Connector)
                return nextItems[0];
        }
        Debug.LogError("node with connector not found");
        return null;
    }

    private SpawnNode RandomFindFarEmptyNodeFrom(int maxCountBetweenNodes, int minCountBetweenNodes, string nameForDebug, params SpawnNodeType[] types)
    {
        for (int i = maxCountBetweenNodes; minCountBetweenNodes > 0; i--)
        {
            for (int j = 0; j < 100; j++)
            {
                var node = RandomUtils.GetRandomItem(_emptyNodes);
                int dist = GetNearNodeDistance(node, types);
                if (dist > i && node.NodeType == SpawnNodeType.Empty)
                {
                    return node;
                }
            }
            //Debug.LogWarning(nameForDebug + ": not found node with maxCountBetweenNodes=" + i);
        }
        return null;
    }

    private void RemoveNodeFromEmptyNodes(SpawnNode node, SpawnNodeType newType)
    {
        node.NodeType = newType;
        if (newType == SpawnNodeType.Connector && node.DirectionInCover!=Direction.None)
            node.NodeType = SpawnNodeType.CornerConnector;
        _emptyNodes.Remove(node);
    }

    private void DebugEmptyNodes()
    {
        foreach (var node in _allNodes) //_emptyNodes)
        {
            if (node.NodeType == SpawnNodeType.Empty)
                Debug.DrawLine(node.GridNode.Position, node.GridNode.Position + Vector3.up*2, Color.red, 100f);
        }
    }

    /// <summary>
    /// Добавление нод в список против часовой от ноды справа от двери [6,0]
    /// </summary>
    private void FillNodes()
    {
        int index = 0;

        for (int i = 6; i >= 0; i--)
            _allNodes.Add(new SpawnNode(NodesGrid.Grid[i, 0], index++));

        for (int i = 1; i < 7; i++)
            _allNodes.Add(new SpawnNode(NodesGrid.Grid[0, i], index++));

        for (int i = 1; i < 7; i++)
            _allNodes.Add(new SpawnNode(NodesGrid.Grid[i, 6], index++));

        for (int i = 5; i >= 1; i--)
            _allNodes.Add(new SpawnNode(NodesGrid.Grid[6, i], index++));

        _emptyNodes.AddRange(_allNodes);
        RemoveNodeFromEmptyNodes(_allNodes[1], SpawnNodeType.Busy);
    }


    private List<SpawnNode> GetEmptyCornerNodes(List<SpawnNode> nodes)
    {
        return nodes.Where(n => n.DirectionInCover != Direction.None && n.NodeType == SpawnNodeType.Empty).ToList();
    }
}
