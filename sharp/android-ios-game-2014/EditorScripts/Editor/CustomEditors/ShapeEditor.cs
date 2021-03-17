using Shapes;
using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof (Shape), true)]
public class ShapeEditor : Editor
{
    private SerializedProperty currentDirection;
    private GameObject obj;
    private Shape shape;

    public void OnEnable()
    {
        obj = Selection.activeGameObject;
        shape = obj.GetComponent<Shape>();

        currentDirection = serializedObject.FindProperty("_currentDirection");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "_currentDirection", "Xindex", "Yindex");

        shape.UpdateCurrentDirectionInEditorMode();
        var index = currentDirection.enumValueIndex;
        EditorGUILayout.LabelField("Direction: ", currentDirection.enumNames[index]);

        if (EditorApplication.isPlaying)
        {
            EditorGUILayout.LabelField("side[Up]: ", shape.Up.ToString());
            EditorGUILayout.LabelField("side[Right]: ", shape.Right.ToString());
            EditorGUILayout.LabelField("side[Down]: ", shape.Down.ToString());
            EditorGUILayout.LabelField("side[Left]: ", shape.Left.ToString());

            EditorGUILayout.LabelField("X index: ", shape.Xindex.ToString());
            EditorGUILayout.LabelField("Y index: ", shape.Yindex.ToString());
        }

        if (GUILayout.Button("RotateTolLeft"))
            shape.RotateThroughInspector();

        
        serializedObject.ApplyModifiedProperties();
    }
}