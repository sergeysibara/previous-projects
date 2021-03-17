using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;
using Shapes;
using Random = UnityEngine.Random;

public partial class RoomContentGenerator
{

    private void CreateSofa(out Vector3 position, out Direction sofaDir, out int xIndex, out int yIndex)
    {
        sofaDir = (Direction)Random.Range(0, 4); //sofaDir = Direction.Left;
        Quaternion sofaQuaternion = DirectionUtils.DirectionToQuaternion(sofaDir);
        xIndex = 0;
        yIndex = 0;

        switch (sofaDir)
        {
            case Direction.Up:
                xIndex = Random.Range(2, 4);
                yIndex = Random.Range(3, 5);
                break;

            case Direction.Down:
                xIndex = 2;
                yIndex = Random.Range(2, 4);
                break;

            case Direction.Right:
                xIndex = Random.Range(3, 5);
                yIndex = Random.Range(2, 4);
                break;

            case Direction.Left:
                xIndex = Random.Range(2, 4);
                yIndex = Random.Range(2, 4);
                break;
        }

        NodesGrid.Node node = NodesGrid.Grid[xIndex, yIndex];
        position = node.Position;

        var sofa = (Transform)Instantiate(_sofaPrefab, position, sofaQuaternion);
        GridAligner.AlignTwoNodeObject(sofa);
        sofa.parent = _furniture;
    }

    private void CreatePlasma(Vector3 sofaPos, Direction sofaDir, out Vector3 plasmaPos)
    {
        Vector3 position = sofaPos;
        position.y = _plasmaTVPrefab.transform.position.y + 10f;
        Direction dir = sofaDir.GetOpposite();

        switch (dir)
        {
            case Direction.Up:
            case Direction.Down:
                position.z -= 100f * dir.CreateSign();
                position.x += 0.5f;
                break;

            case Direction.Right:
            case Direction.Left:
                position.z += 0.5f;
                position.x -= 100f * dir.CreateSign();
                break;
        }
        var plasmaTV_pos = plasmaPos = RoomClamp(position);

        var plasmaTV = (Transform)Instantiate(_plasmaTVPrefab, plasmaTV_pos, DirectionUtils.DirectionToQuaternion(dir));
        plasmaTV.parent = _devices;
    }

    private void CreatePlasmaConnector(Vector3 plasmaPos, Direction sofaDir)
    {
        Vector3 position = plasmaPos;
        position.y = _connectorPrefab.position.y + 10f;
        Direction dir = sofaDir.GetOpposite();
        int secondNodeOffsetIndexX = 0;
        int secondNodeOffsetIndexY = 0;

        switch (dir)
        {
            case Direction.Up:
            case Direction.Down:
                position.x -= 0.5f;
                secondNodeOffsetIndexX = 1;
                break;

            case Direction.Right:
            case Direction.Left:
                position.z -= 0.5f;
                secondNodeOffsetIndexY = 1;
                break;
        }

        var plasmaConnector = (Transform)Instantiate(_connectorPrefab, position, DirectionUtils.DirectionToQuaternion(dir));
        plasmaConnector.parent = SceneContainers.Connectors;

        var nearNode = plasmaConnector.GetComponent<Connector>().NearestNode;
        var plasmaConnectorSpawnNode = _emptyNodes.Find(c => c.GridNode == NodesGrid.Grid[nearNode.X, nearNode.Y]);
        RemoveNodeFromEmptyNodes(plasmaConnectorSpawnNode, SpawnNodeType.Connector);

        var secondNode = _emptyNodes.Find(c => c.GridNode == NodesGrid.Grid[nearNode.X + secondNodeOffsetIndexX, nearNode.Y + secondNodeOffsetIndexY]);
        RemoveNodeFromEmptyNodes(secondNode, SpawnNodeType.Device);
    }

    private void CreateStartConnector(Direction sofaDirection)
    {
        int spawnIndex;
        if (sofaDirection == Direction.Down)
            spawnIndex = 0;
        else if (sofaDirection == Direction.Right)
            spawnIndex = 1;
        else
            spawnIndex = Random.Range(0, 2);

        var spawnNode = _emptyNodes[spawnIndex];
        var startConnector_pos = spawnNode.GridNode.AstarNode.BottomCenterPosition();
        startConnector_pos.y = _connectorPrefab.position.y + 10f;

        var startConnector = (Transform)Instantiate(_connectorPrefab, startConnector_pos, Quaternion.LookRotation(Vector3.forward));
        startConnector.parent = SceneContainers.Connectors;
        startConnector.GetComponent<Connector>().IsStartConnector = true;

        RemoveNodeFromEmptyNodes(spawnNode, SpawnNodeType.Connector);
    }

    private void CreateConnector(SpawnNode spawnNode, Direction direction)
    {
        var startConnector_pos = spawnNode.GridNode.AstarNode.position.ToVector3();
        startConnector_pos.y = _connectorPrefab.position.y + 10f;

        switch (direction)
        {
            case Direction.Up:
            case Direction.Down:
                startConnector_pos.z -= 0.5f*direction.CreateSign();
                break;

            case Direction.Left:
            case Direction.Right:
                startConnector_pos.x -= 0.5f * direction.CreateSign();
                break;
        }

        var connector = (Transform)Instantiate(_connectorPrefab, startConnector_pos, DirectionUtils.DirectionToQuaternion(direction));
        connector.parent = SceneContainers.Connectors;

        RemoveNodeFromEmptyNodes(spawnNode, SpawnNodeType.Connector);
    }

    private void CreateWindows(Direction sofaDir)
    {
        var prefab = RandomUtils.GetRandomItem(_windowPrefabs);
        int winCount = (Random.Range(1, 4) == 3) ? 1 : 2;
        //winCount = 2;//Test only

        int prevDir = -1;
        for (int i = 0; i < winCount; i++)
        {
            Direction dir = (Direction)RandomUtils.RangeWithExclude(0, 4, (int)sofaDir.GetOpposite(), prevDir);
            prevDir = (int) dir;

            Vector3 pos = new Vector3(0f, prefab.position.y + 10f, 0f);
            switch (dir)
            {
                case Direction.Up:
                    pos.x = localOffset - 0.5f;//0.65f;
                    pos.z = -100f;
                    break;

                case Direction.Down:
                    pos.x = localOffset - 0.5f + Random.Range(0, 2);
                    pos.z = 100f;
                    break;

                case Direction.Right:
                case Direction.Left:
                    pos.x = -100 * dir.CreateSign();
                    pos.z = localOffset - 0.5f + Random.Range(0, 2);
                    break;
            }

            pos = RoomClamp(pos);
            var window = (Transform)Instantiate(prefab, pos, DirectionUtils.DirectionToQuaternion(dir));
            window.parent = _windows;

            var nearNode = AstarPath.active.astarData.gridGraph.GetNearest(pos - new Vector3(0.1f, 0, 0.1f)).node;
            var spawnNode = _emptyNodes.Find(c => c.GridNode.AstarNode == nearNode);
            RemoveNodeFromEmptyNodes(spawnNode, SpawnNodeType.Busy);

            var nearNode2 = AstarPath.active.astarData.gridGraph.GetNearest(pos + new Vector3(0.1f, 0, 0.1f)).node;
            var spawnNode2 = _emptyNodes.Find(c => c.GridNode.AstarNode == nearNode2);
            RemoveNodeFromEmptyNodes(spawnNode2, SpawnNodeType.Busy);
        }
    }


    private void CreateComp()
    {
        SpawnNode compSpawnNode = RandomFindFarEmptyNodeFrom(5, 3, "Comp",SpawnNodeType.Connector, SpawnNodeType.CornerConnector);
        if (compSpawnNode==null)
            compSpawnNode = GetFarEmptyNodeFrom(SpawnNodeType.Connector, SpawnNodeType.CornerConnector);

        var compPrefab = RandomUtils.GetRandomItem(_compPrefabs);
        var comp = (Transform)Instantiate(compPrefab, compSpawnNode.GridNode.Position, DirectionUtils.DirectionToQuaternion(compSpawnNode.MainDirection));
        comp.parent = _devices;

        RemoveNodeFromEmptyNodes(compSpawnNode, SpawnNodeType.Device);

        var connectorSpawnNode = GetFarthestFromConnectorstNeighborNode(compSpawnNode);
        Direction connectorDir = compSpawnNode.DirectionInCover == Direction.None ? compSpawnNode.MainDirection : connectorSpawnNode.MainDirection;
        CreateConnector(connectorSpawnNode, connectorDir);
    }

    private void CreatePhone()
    {
        //spawnNode = GetEmptyCornerNodes(_emptyNodes).First(); //TEST only 
        SpawnNode spawnNode = RandomFindFarEmptyNodeFrom(5, 3, "Phone", SpawnNodeType.Connector, SpawnNodeType.CornerConnector);
        if (spawnNode == null)
            spawnNode = GetFarEmptyNodeFrom(SpawnNodeType.Connector, SpawnNodeType.CornerConnector);

        Vector3 pos = spawnNode.GridNode.Position;
        pos.y = _phonePrefab.position.y + 10f;
        switch (spawnNode.MainDirection)
        {
            case Direction.Up:
            case Direction.Down:
                pos.z -= 0.5f * spawnNode.MainDirection.CreateSign();
                break;

            case Direction.Left:
            case Direction.Right:
                pos.x -= 0.5f * spawnNode.MainDirection.CreateSign();
                break;
        }

        var phone = (Transform)Instantiate(_phonePrefab, pos, DirectionUtils.DirectionToQuaternion(spawnNode.MainDirection));
        phone.parent = _devices;

        //SpawnNodeType.Device  - не нужно
        CreateConnector(spawnNode, spawnNode.MainDirection);
    }

    private void CreateCovers(int targetCount, Transform prefab)
    {
        int count = 0;
        var cornerNodes = GetEmptyCornerNodes(_emptyNodes);
        foreach (var spawnNode in cornerNodes)
        {
            count++;
            if (count > targetCount)
                return;
            InstantiateCover(prefab, spawnNode);
        }

        for (int i = 0; i < targetCount; i++)
        {
            SpawnNode spawnNode = null;
            for (int j = 0; j < 100; j++)
            {
                var node = RandomUtils.GetRandomItem(_emptyNodes);
                int dist = GetNearNodeDistance(node, SpawnNodeType.Cover);
                if (dist > _nodesBetweenCorners)
                {
                    dist = GetNearNodeDistance(node, SpawnNodeType.CornerConnector);
                    if (dist > 1)
                    {
                        spawnNode = node;
                        break;
                    }
                }
            }

            if (spawnNode == null)
            {
                //Debug.LogWarning(prefab.name + ": not found node with maxCountBetweenNodes=" + _nodesBetweenCorners);
                return;
            }

            count++;
            if (count > targetCount)
                return;
            InstantiateCover(prefab, spawnNode);
        }
    }

    private void InstantiateCover(Transform prefab, SpawnNode spawnNode)
    {
        Direction dir = spawnNode.MainDirection;

        Vector3 pos = spawnNode.GridNode.Position;
        pos.y += 0.22f;
        var cover = (Transform)Instantiate(prefab, pos, DirectionUtils.DirectionToQuaternion(dir));
        cover.parent = _furniture;

        RemoveNodeFromEmptyNodes(spawnNode, SpawnNodeType.Cover);
    }


    //private void SetCatPosition()
    //{
    //    var node = GetNearNode(_allNodes[6], SpawnNodeType.Cover, SpawnNodeType.Empty);
    //    _cat.position.x=
    //    _cat.position.z
    //}
}
