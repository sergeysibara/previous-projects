#if UNITY_EDITOR
//копирование частично взято из AddBlendTreeCallback в BlendTreeInspector.cs
//Дочерние клипы просто копируются, поэтому их надо заменять вручную, если необходимо
//Также у родительского стейта придется устанавливать значение Threshold (thres). Можно было бы конечно как-нибудь найти текущий контроллер и родительский стейт, но неохота реализовывать

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// Добавляет возможность рекурсивного копирования и вставки BlendTree стейтов и клипов 
/// </summary>
public class BlendTreeCopier
{
    static Motion originalState;
    const bool add_clone_TextToChildBlendTrees = false; //при true будет добавлять "clone" к дочерним blendTree-s копируемого blendTree

    [MenuItem("Tools/Copy Mecanim motion from blendTree",false,10)]
    static void Copy()
    {

        Debug.LogWarning(Selection.activeObject.GetType());
         //if (Selection.activeObject is Motion)
         //   {
         //       Debug.Log(Selection.activeObject.name);
         //       originalState = (Motion)Selection.activeObject;
         //   }
    }

    [MenuItem("Tools/Paste Mecanim motion in blendTree", false, 11)]
    static void Paste()
    {
        //if (originalState != null && Selection.activeObject is BlendTree)
        //{
        //    var blendTreeOriginal = originalState as BlendTree;
        //    if (blendTreeOriginal != null)
        //    {
        //        BlendTree stateClone = BlendTreeClone(blendTreeOriginal, false);
        //         AssetDatabase.AddObjectToAsset(stateClone, Selection.activeObject);
        //        ((BlendTree)Selection.activeObject).AddMotion(stateClone);
        //        return;
        //    }

        //    var motion = originalState as Motion;
        //    if (motion != null)
        //    {
        //        ((BlendTree)Selection.activeObject).AddMotion(motion);
        //        return;
        //    }
        //}
        //Debug.LogWarning("нельзя вставить Motion в текущий стейт");
    }

    /// <param name="isChild">является ли blendTree дочерним в клонированном BlendTree</param>
    static BlendTree BlendTreeClone(BlendTree original, bool isChild)
    {
        //BlendTree stateClone = new BlendTree
        //{
        //    hideFlags = HideFlags.HideInHierarchy,
        //    name = original.name
        //};
        //if (!isChild || (isChild && add_clone_TextToChildBlendTrees))
        //    stateClone.name += " clone";
        //stateClone.SetBlendType(original.GetBlendType());
        //stateClone.SetUseAutomaticThresholds(original.GetUseAutomaticThresholds());
        //stateClone.SetMaxThreshold(original.GetMaxThreshold());
        //stateClone.SetMinThreshold(original.GetMinThreshold());
        //stateClone.SetBlendEvent(original.GetBlendEvent());
        //stateClone.SetBlendEventY(original.GetBlendEventY());

        //for (int i = 0; i < original.GetChildCount(); i++)
        //{
        //    Motion childMotion = original.GetMotion(i);
        //    if (childMotion is BlendTree)
        //        childMotion = BlendTreeClone((BlendTree)childMotion, true);

        //    stateClone.AddMotion(childMotion);
        //    stateClone.SetChildTimeScale(i, original.GetChildTimeScale(i));
        //    stateClone.SetChildTreshold(i, original.GetChildTreshold(i));
        //}
        //return stateClone;
        return null;
    }
}
#endif
