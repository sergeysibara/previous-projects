using UnityEngine;
using System.Collections;
using Pathfinding;

public static class PositionChecker
{
    public static bool NodeIsWalkable(Node node)
    {
        return (node.tags != (int)GridTags.Buildings && node.walkable);
    }

    public static bool NodePositionIsFree(Node[] nodes, int nodeIndex, out Vector3 nodePos)
    {
        if (nodeIndex >= 0 && nodeIndex < nodes.Length && NodeIsWalkable(nodes[nodeIndex]))
        {
            Vector3 newNodePos = nodes[nodeIndex].position.ToVector3();
            if (PositionIsFree(newNodePos, 0.5f))
            {
                nodePos = newNodePos;
                return true;
            }
        }
        nodePos = Vector3.zero;
        return false;
    }

    public static bool PositionIsFree(Vector3 point, float radius)
    {
        Collider[] colls = Physics.OverlapSphere(point, radius, GameManager.AllObjectLayers);
        if (colls.Length > 0)
            return false;
        return true;
    }

    /// <param name="position">глобальная позиция</param>
    public static Vector3 FindNearestFreePosition(Vector3 position)
    {
        Node defaultSpawnNode = AstarPath.active.GetNearest(position);

        if (PositionChecker.NodeIsWalkable(defaultSpawnNode) && PositionChecker.PositionIsFree(position, 0.5f))
            return position;


        int width = AstarPath.active.astarData.gridGraph.width;
        Node[] nodes = AstarPath.active.astarData.gridGraph.nodes;
        int centerNodeIndex = defaultSpawnNode.GetNodeIndex();
        int index;
        int indexX;//используется для провеки выхода индекса за границы диапазона 0..GridWidth
        int columnHeight = 0, rowWidth = 0;

        //(спиральный поиск по часовой) 
        for (int i = 0; i < 100; i++)
        {
            columnHeight++;

            indexX = centerNodeIndex % width + i + 1;
            if (indexX < width)
            {
                for (int j = -columnHeight; j <= columnHeight; j++)
                {
                    index = centerNodeIndex - j * width + i + 1;
                    Vector3 nodePos;
                    if (PositionChecker.NodePositionIsFree(nodes, index, out nodePos))
                        return nodePos;
                }
            }


            for (int j = -rowWidth; j <= rowWidth; j++)
            {
                indexX = centerNodeIndex % width - j;
                if (indexX >= 0 && indexX < width)
                {
                    index = centerNodeIndex - (i + 1) * width - j;
                    Vector3 nodePos;
                    if (PositionChecker.NodePositionIsFree(nodes, index, out nodePos))
                        return nodePos;
                }
            }

            indexX = centerNodeIndex % width - i - 1;
            if (indexX >= 0)
            {
                for (int j = -columnHeight; j <= columnHeight; j++)
                {
                    index = centerNodeIndex + j * width - i - 1;
                    Vector3 nodePos;
                    if (PositionChecker.NodePositionIsFree(nodes, index, out nodePos))
                        return nodePos;
                }
            }

            for (int j = -rowWidth; j <= rowWidth; j++)
            {
                indexX = centerNodeIndex % width + j;
                if (indexX >= 0 && indexX < width)
                {
                    index = centerNodeIndex + (i + 1) * width + j;
                    Vector3 nodePos;
                    if (PositionChecker.NodePositionIsFree(nodes, index, out nodePos))
                        return nodePos;
                }
            }

            rowWidth++;
        }
        return position;
    }
}
