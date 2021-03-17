using UnityEditor;
using System.Collections;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(BlendTree), true)]
public class StateEditor : Editor 
{
        void OnEnable()
	{
	}

	public override void OnInspectorGUI()
	{
	    if (GUILayout.Button("RotateTolLeft"))
	    {

	    }
	}
}
