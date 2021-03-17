using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class BuildingGrid : MonoBehaviourHeritor
{
    public bool IsGhost;
    public int CurrentCollisionAmount;

    Node[] _lastNodes;
    Node[] _boundaryNodes; //���� �� �������� ����������. ������������ ��� ������ ����
    bool _isShuttingDown = false;

    public Node[] GetNodes()
    {
        if (_lastNodes == null)
            FindNodes();

        return _lastNodes;
    }

    public Node[] GetNodesClone()
    {
        return (Node[])GetNodes().Clone();
    }

    public Node[] GetBoundaryNodes()
    {
        if (_boundaryNodes == null)
            FindBoundaryNodes();

        return _boundaryNodes;
    }

    public Node[] GetBoundaryNodesClone()
    {
        return (Node[])GetBoundaryNodes().Clone();
    }

    public void Start()
    {
        if (!IsGhost)
        {
            DoUpdateGraphs(GridTags.Buildings);
        }
    }


    /** Revert graphs when destroyed.
     * When the object is destroyed, a last graph update should be done to revert nodes to their original state */
    void OnDestroy()
    {
        if (!IsGhost && !_isShuttingDown)
        {
            DoUpdateGraphs(GridTags.BasicGround);
        }
    }

    void OnApplicationQuit()
    {
        _isShuttingDown = true;
    }


    #region �������� �������� � ������ ���������� �������
    void FixedUpdate()
    {
        if (IsGhost)
        {
            CurrentCollisionAmount = 0;
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (IsGhost)
        {
            //Debug.Log("stay");
            if (!GameManager.GroundLayers.IsLayerInLayerMask(col.gameObject.layer))
            //&& GameManager.DefaultLayer != col.gameObject.layer - �� �����, �.�. ������������ � ���������� ������
            {
                CurrentCollisionAmount++;
            }
        }
    }
    #endregion


    //��������� ���� ��� ������� ������������ ���� ���, ��� ���������� ������� �������
    public void DoUpdateGraphs(GridTags tag)
    {
        foreach (Node node in GetNodes())
            node.tags = (int)tag;
    }

    #region Find nodes
    /* FindNodes() � FindBoundaryNodes() �������, ��� ����� ��������� ��������� � ������ ����������� ���� �������. 
     * � ����� � ���� �������� ���:
     * "���� ����� ���������� �� ��������� X <= 1, �� �������� ���������� �� � ������ ����, �� ������� ������ ������ ����, ������ ���� ������� ����������� ���������"  
     * ������� ����� ����������� ��������� � ������ �������, � ����� ������� �� ���, ����� ���� �������� ��������� ��� ��������� �������.
     * 
     * ���� ������ ������ � ������������� �������, ������� ���� ������ �� ������������� ��� �� ��������, �� ����� �������� ������ ����. 
     * �� ����� ������ � ������� �������� overlapSphere � �������� 0.5 � ������ ���� � ��������, ��� ��������� ��������� ����������� ����� �������. */ 

    /// <summary>
    /// ����� ��� ���, ���������� � ������������� ������� ����������, � ����� ��������� ��������� ��� � ���� _prevNodes.
    /// </summary>
    public Node[] FindNodes()
    {
        Bounds bounds = collider.bounds;
        float xMin = bounds.min.x;
        float zMin = bounds.min.z;
        float xMax = bounds.max.x;
        float zMax = bounds.max.z;
        
        List<Node> newNodes = new List<Node>();

        for (float x = xMin; x <= xMax; x += GameManager.NodeSize)
        {
            for (float z = zMin; z <= zMax; z += GameManager.NodeSize)
            {
                newNodes.Add(AstarPath.active.GetNearest(new Vector3(x, bounds.min.y, z)));
                //Node node = AstarPath.active.GetNearest(new Vector3(x, 0, z));
                //Debug.Log(node.GetNodeIndex());
            }
        }

        if (newNodes.Count == 0)
            Debug.LogError("Nodes not found");

        _lastNodes = newNodes.ToArray();
        return _lastNodes;
    }

    /// <summary>
    /// ����� ��� ���, �� �������� ������������� ������� ����������, � ����� ��������� ��������� ��� � ���� _boundaryNodes.
    /// </summary>
    void FindBoundaryNodes()
    {
        Bounds bounds = collider.bounds;
        float xMin = bounds.min.x;
        float zMin = bounds.min.z;
        float xMax = bounds.max.x;
        float zMax = bounds.max.z;

        List<Node> newNodes = new List<Node>();

        for (float x = xMin; x <= xMax; x += GameManager.NodeSize)
        {
            for (float z = zMin; z <= zMax; z += GameManager.NodeSize)
            {
                if (x == xMin || x > xMax - GameManager.NodeSize || z == zMin || z > zMax - GameManager.NodeSize)
                {
                    newNodes.Add(AstarPath.active.GetNearest(new Vector3(x, bounds.min.y, z)));
                }
            }
        }

        if (newNodes.Count == 0)
            Debug.LogError("Nodes not found");

        _boundaryNodes = newNodes.ToArray();
    }
    #endregion

    //����������� ���, � ������� �������� ������
    void OnDrawGizmos()
    {
        if (_lastNodes != null)
        {
            Gizmos.color = new Color(1.0f, 0.5f, 0.0f, 0.5f);
            float nodeSize = GameManager.NodeSize * 0.97f;
            Vector3 cubeSize = new Vector3(nodeSize, nodeSize, nodeSize);

            foreach (Node node in _lastNodes)
                if (node.tags==(int)GridTags.Buildings)
                    Gizmos.DrawCube(node.position.ToVector3(), cubeSize);
        }
    }

}
