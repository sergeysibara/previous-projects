using UnityEditor;

[CustomEditor(typeof(PlayerBulletsLayerDDL))]
public class PlayerBulletsLayerDDL_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        PlayerBulletsLayerDDL myTarget = target as PlayerBulletsLayerDDL;
        myTarget.Index = EditorGUILayout.Popup("Player bullets Layer", myTarget.Index, UnityEditorInternal.InternalEditorUtility.layers);
        myTarget.Layer = UnityEditorInternal.InternalEditorUtility.layers[myTarget.Index];
	}
}
