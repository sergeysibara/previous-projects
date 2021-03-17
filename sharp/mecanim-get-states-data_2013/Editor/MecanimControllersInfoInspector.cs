using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

[CustomEditor(typeof(MecanimControllersInfo))]
public class MecanimControllersInfoInspector : Editor
{

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Update states data"))
        {
            MecanimControllersInfo controllersInfo = serializedObject.targetObject as MecanimControllersInfo;
		    ExtractStatesData(controllersInfo);
            EditorUtility.SetDirty(serializedObject.targetObject);
        }

        DrawDefaultInspector();
    }

    void ExtractStatesData(MecanimControllersInfo controllersInfo)
    {
        controllersInfo.ControllersData = null;
        List<MecanimControllerDataEntry> controllerDataList = new List<MecanimControllerDataEntry>();

        foreach (var controller in controllersInfo.AnimatorControllers)
        {
            if (controller == null)
                continue;

            List<MecanimStateDataEntry> stateDataList = new List<MecanimStateDataEntry>();
            int layerCount = controller.GetLayerCount();

            for (int layer = 0; layer < layerCount; layer++)
            {
                StateMachine targetStateMachine = controller.GetLayerStateMachine(layer);
                List<State> states = targetStateMachine.statesRecursive;
                stateDataList.AddRange(states.Select(st => new MecanimStateDataEntry(layer, st.name, st.GetUniqueName(), st.GetTag(),
                                                                                     st.GetUniqueNameHash(), Animator.StringToHash(st.GetTag())
                                                                                     )));
            }

            MecanimControllerDataEntry controllerData = new MecanimControllerDataEntry();
            controllerData.Controller = controller;
            controllerData.StatesData = stateDataList.ToArray();

            controllerDataList.Add(controllerData);
        }

        controllersInfo.ControllersData = controllerDataList.ToArray();
        Debug.Log("Данные MecanimControllersInfo обновлены");
    }

}
