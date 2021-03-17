using UnityEditor;

[CustomEditor(typeof(PlayerObjectsLayerDDL))]
public class PlayerObjectsLayerDDL_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        PlayerObjectsLayerDDL myTarget = target as PlayerObjectsLayerDDL;
        myTarget.Index = EditorGUILayout.Popup("Player objects Layer", myTarget.Index, UnityEditorInternal.InternalEditorUtility.layers);
        myTarget.Layer = UnityEditorInternal.InternalEditorUtility.layers[myTarget.Index];
	}
}
