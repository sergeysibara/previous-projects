using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditorInternal;
#endif

public class MecanimControllersInfo : MonoBehaviour
{
#if UNITY_EDITOR
    /// <summary>
    /// ����������� �� ������� ����������� ���������� �� �� ������ �������.
    /// </summary>
    public List<AnimatorController> AnimatorControllers = new List<AnimatorController>();
#endif

    public MecanimControllerDataEntry[] ControllersData;

    /// <summary>
    /// ����� ������ �� ���� �����
    /// </summary>
    /// <param name="layer">����� ����. ��� �������� -1, ����� ���� �� ����������� ��� ������</param>
    public MecanimStateDataEntry[] GetStatesDataByNameHash(int controllerIndex, int nameHash, int layer=-1)
    {
        if (controllerIndex >= ControllersData.Length)
        {
            Debug.LogError("controllerIndex Out Of Range");
            return null;
        }
        if (layer<0)
            return Array.FindAll(ControllersData[controllerIndex].StatesData, std => std.UniqueNameHash == nameHash);
        else
            return Array.FindAll(ControllersData[controllerIndex].StatesData, std => std.UniqueNameHash == nameHash && std.Layer == layer);
    }

    /// <summary>
    /// ����� ������ �� ���� ����
    /// </summary>
    /// <param name="layer">����� ����. ��� �������� -1, ����� ���� �� ����������� ��� ������</param>
    public MecanimStateDataEntry[] GetStatesDataByTagHash(int controllerIndex, int tagHash, int layer = -1)
    {
        if (controllerIndex >= ControllersData.Length)
        {
            Debug.LogError("controllerIndex Out Of Range");
            return null;
        }
        if (layer < 0)
            return Array.FindAll(ControllersData[controllerIndex].StatesData, std => std.TagHash == tagHash);
        else
            return Array.FindAll(ControllersData[controllerIndex].StatesData, std => std.TagHash == tagHash && std.Layer == layer);
    }
}


#region ������ ��� �������� ������

[System.Serializable]
public class MecanimControllerDataEntry
{
    public UnityEngine.Object Controller; 
    public MecanimStateDataEntry[] StatesData;
}

[System.Serializable]
public class MecanimStateDataEntry
{
    //��� ���� �����������, �.� ��� ����� �������� ����� ��������: Animator a; a.GetLayerName(layerIndex)
    public int Layer;
    public string Name;
    public string UniqueName;
    public string Tag;
    public int UniqueNameHash;
    public int TagHash;

    public MecanimStateDataEntry(int layer, string name, string uniqueName, string tag, int uniqueNameHash, int tagHash)
    {
        Layer = layer;
        Name = name;
        UniqueName = uniqueName;
        Tag = tag;
        UniqueNameHash = uniqueNameHash;
        TagHash = tagHash;
    }

    /// <summary>
    /// ���������� ���������� ����� �����. ��� ����� ����� �������� ��� ������ ����, ��� � ������ ������   
    /// </summary>
    public string GetUniquePartOfName()
    {
        int index = UniqueName.IndexOf('.');
        if (index != -1)
            return UniqueName.Remove(index);
        return UniqueName;
    }
}
#endregion