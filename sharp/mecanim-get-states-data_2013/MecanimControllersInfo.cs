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
    /// Контроллеры из которых извлекается информация об их наборе стейтов.
    /// </summary>
    public List<AnimatorController> AnimatorControllers = new List<AnimatorController>();
#endif

    public MecanimControllerDataEntry[] ControllersData;

    /// <summary>
    /// Поиск стейта по хэшу имени
    /// </summary>
    /// <param name="layer">Номер слоя. При значении -1, номер слоя не учитывается при поиске</param>
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
    /// Поиск стейта по хэшу тега
    /// </summary>
    /// <param name="layer">Номер слоя. При значении -1, номер слоя не учитывается при поиске</param>
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


#region Классы для хранения данных

[System.Serializable]
public class MecanimControllerDataEntry
{
    public UnityEngine.Object Controller; 
    public MecanimStateDataEntry[] StatesData;
}

[System.Serializable]
public class MecanimStateDataEntry
{
    //имя слоя отсутствует, т.к его можно получить через аниматор: Animator a; a.GetLayerName(layerIndex)
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
    /// Возвращает уникальную часть имени. Эта часть может являться как именем слоя, так и именем группы   
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