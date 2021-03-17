using UnityEngine;
using System.Collections;
using Pathfinding;
public static class AstarPathUtils
{ 
    #if UNITY_EDITOR
    /// <summary>
    /// Editor only!!!
    /// </summary>
    public static void CreateGraphIfItNull(AstarPath path)
    {
        if (path == null)
        {
            Debug.LogWarning("AstarPath not found");
            return;
        }
        if (path.astarData == null || path.astarData.gridGraph == null)
            AstarPath.MenuScan();
    }
    #endif
}

public static class GraphNodeExt
{
    public static Vector3 BottomCenterPosition(this GraphNode node)
    {
        var pos = node.position.ToVector3();
        pos.z -= AstarPath.active.astarData.gridGraph.nodeSize*0.5f;
        return pos;
    }

}