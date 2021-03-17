
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using System.Linq;

[ExecuteInEditMode]
public class GridAligner : MonoBehaviour
{
#if UNITY_EDITOR
    #region Inspector variables
    //обзначения имен в этом регионе:
    //Containers - список родителей, прямые дети которых будут выравниваться.
    //Objects - спискок конкретных объектов, которые будут выравниваться.
    [SerializeField]
    private AstarPath _astarPath;

    [SerializeField]
    private float _yPos = 0.3f;

    [SerializeField]
    private List<Transform> ContainersForAligningByXYZ;

    [SerializeField]
    private List<Transform> ObjectsForAligningByXYZ;

    [SerializeField]
    private List<Transform> ContainersForAligningByXZ;

    [SerializeField]
    private List<Transform> ObjectsForAligningByXZ;

    [SerializeField]
    private List<Transform> ContainersForTwoNodeObjectsByXZ;
    #endregion

    private void Start()
    {
    }

    private void Update()
    {
        if (Application.isPlaying || _astarPath==null)
            return;

        AstarPathUtils.CreateGraphIfItNull(_astarPath);

        AlignPositionByXYZ(GetListForAligning(ContainersForAligningByXYZ, ObjectsForAligningByXYZ));
        AlignPositionByXZ(GetListForAligning(ContainersForAligningByXZ, ObjectsForAligningByXZ));
        AlignPositionTwoNodeObjectsByXZ(GetListForAligning(ContainersForTwoNodeObjectsByXZ, null));
    }

    private IEnumerable<Transform> GetListForAligning(IEnumerable<Transform> Containers, IEnumerable<Transform> concreteObjects)
    {
        List<Transform> allSelectedChilds = new List<Transform>();

        foreach (Transform child in Containers)
            allSelectedChilds.AddRange(child.Cast<Transform>());

        if (concreteObjects != null)
            allSelectedChilds.AddRange(concreteObjects);
        return allSelectedChilds;
    }

    private void AlignPositionByXYZ(IEnumerable<Transform> transforms)
    {
        var gridGraph = AstarPath.active.astarData.gridGraph;
        foreach (var tr in transforms)
        {
            Vector3 newPos = gridGraph.GetNearest(tr.position).node.position.ToVector3();
            newPos.y += _yPos;
            tr.position = newPos;
        }
    }

    private void AlignPositionByXZ(IEnumerable<Transform> transforms)
    {
        var gridGraph = AstarPath.active.astarData.gridGraph;
        foreach (var tr in transforms)
        {
            Vector3 newPos = gridGraph.GetNearest(tr.position).node.position.ToVector3();
            newPos.y = tr.position.y;
            tr.position = newPos;
        }
    }

    private void AlignPositionTwoNodeObjectsByXZ(IEnumerable<Transform> transforms)
    {
        foreach (var tr in transforms)
            AlignTwoNodeObject(tr);
    }
#endif

    public static void AlignTwoNodeObject(Transform tr)
    {
        var gridGraph = AstarPath.active.astarData.gridGraph;
        Vector3 newPos = gridGraph.GetNearest(tr.position).node.position.ToVector3() - new Vector3(0.01f, 0f, 0.01f);
        newPos.y = tr.position.y;
        var angle = tr.eulerAngles.y;

        switch (Mathf.RoundToInt(angle))
        {
            case 0:
            case 180:
                newPos.x += 0.5f;
                break;
            case 90:
            case 270:
                newPos.z += 0.5f;
                break;
        }
        tr.position = newPos; //+new Vector3(0.1f, 0f, 0.1f);
    }
}
