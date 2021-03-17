using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class BuildingGrid : MonoBehaviourHeritor
{
    public bool IsGhost;
    public int CurrentCollisionAmount;

    Node[] _lastNodes;
    Node[] _boundaryNodes; //ноды на границах коллайдера. Используются при поиске пути
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


    #region Проверка коллизий в режиме размещения объекта
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
            //&& GameManager.DefaultLayer != col.gameObject.layer - не нужно, т.к. игнорируется в настройках физики
            {
                CurrentCollisionAmount++;
            }
        }
    }
    #endregion


    //установка тега для задания проходимости всех нод, где расположен текущий объекта
    public void DoUpdateGraphs(GridTags tag)
    {
        foreach (Node node in GetNodes())
            node.tags = (int)tag;
    }

    #region Find nodes
    /* FindNodes() и FindBoundaryNodes() считают, что центр колладера находится в центре центральной ноды объекта. 
     * В связи с этим возможен баг:
     * "если длина коллайдера по координте X <= 1, но колладер расположен не в центре ноды, то учтется только первая нода, вместо двух которые затрагивает коллайдер"  
     * Поэтому важно распологать коллайдер в центре объекта, а также следить за тем, какие ноды занимает коллайдер при установки объекта.
     * 
     * Ноды ищутся только в прямоугольной области, поэтому если объект не прямоугольный или же повернут, то будут получены лишние ноды. 
     * Их можно убрать с помощью проверки overlapSphere с радиусом 0.5 в каждой ноде и провекой, что найденный коллайдер принадлежит этому объекту. */ 

    /// <summary>
    /// Поиск все нод, попадающих в прямоугольную область коллайдера, а также сохранени найденных нод в поле _prevNodes.
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
    /// Поиск все нод, на границах прямоугольной области коллайдера, а также сохранени найденных нод в поле _boundaryNodes.
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

    //отображение нод, в которых размещен объект
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
