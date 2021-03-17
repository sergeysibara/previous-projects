//Дочерние клипы просто копируются, поэтому их надо заменять вручную, если необходимо
//У родительского стейта придется устанавливать значение Threshold (thres). Можно было бы также сделать определение текущего контроллера и родительского стейта.

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// Добавляет возможность рекурсивного копирования и вставки BlendTree стейтов и клипов 
/// </summary>
public class BlendTreeCopier
{
    private static Motion originalState;
    private const string additionalTextForChilds=" clone"; //добавляет текст к именам дочерних blendTree-s копируемого blendTree

    [MenuItem("Tools/Copy Mecanim motion from blendTree", false, 1)]
    private static void Copy()
    {
        if (Selection.activeObject is Motion)
        {
            Debug.Log("Copy " + Selection.activeObject.name + " motion");
            originalState = (Motion) Selection.activeObject;
        }
        else
            Debug.LogWarning("Selected object is not Motion");
    }

    [MenuItem("Tools/Paste Mecanim motion in blendTree", false, 2)]
    private static void Paste()
    {
        if (originalState != null && Selection.activeObject is BlendTree)
        {
            var blendTreeOriginal = originalState as BlendTree;
            if (blendTreeOriginal != null)
            {
                BlendTree stateClone = BlendTreeClone(blendTreeOriginal, false);
                AssetDatabase.AddObjectToAsset(stateClone, Selection.activeObject);
                ((BlendTree) Selection.activeObject).AddMotion(stateClone);
                return;
            }

            var motion = originalState as Motion;
            if (motion != null)
            {
                ((BlendTree) Selection.activeObject).AddMotion(motion);
                return;
            }
        }
        Debug.LogWarning("Can not paste Motion");
    }

    /// <param name="isChild">является ли blendTree дочерним в клонированном BlendTree</param>
    private static BlendTree BlendTreeClone(BlendTree original, bool isChild)
    {
        BlendTree stateClone = new BlendTree
            {
                hideFlags = HideFlags.HideInHierarchy,
                name = !isChild ? original.name + " clone" : original.name + additionalTextForChilds
            };
        stateClone.SetBlendType(original.GetBlendType());
        stateClone.SetUseAutomaticThresholds(original.GetUseAutomaticThresholds());
        stateClone.SetMaxThreshold(original.GetMaxThreshold());
        stateClone.SetMinThreshold(original.GetMinThreshold());
        stateClone.SetBlendEvent(original.GetBlendEvent());
        stateClone.SetBlendEventY(original.GetBlendEventY());

        for (int i = 0; i < original.GetChildCount(); i++)
        {
            Motion childMotion = original.GetMotion(i);
            if (childMotion is BlendTree)
                childMotion = BlendTreeClone((BlendTree) childMotion, true);

            stateClone.AddMotion(childMotion);
            stateClone.SetChildTimeScale(i, original.GetChildTimeScale(i));
            stateClone.SetChildTreshold(i, original.GetChildTreshold(i));
        }
        return stateClone;
    }
}
#endif
