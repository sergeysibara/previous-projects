
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CustomEditorPrefs
#if UNITY_EDITOR
    : EditorWindow
#endif
{
    private static string GizmoGuiSkinKey
    {
        get
        {
#if UNITY_EDITOR
                return PlayerSettings.productName+"GizmoGuiSkin";
#endif
            return "";
        }
    }



    public static GUISkin GizmoGuiSkin
    {
        get
        {
#if UNITY_EDITOR

            if (EditorPrefs.HasKey(GizmoGuiSkinKey))
            {
                if (_gizmoGuiSkin == null) //_loadedGizmoGuiSkin != _gizmoGuiSkin || _loadedGizmoGuiSkin==null)
                {
                    var path = EditorPrefs.GetString(GizmoGuiSkinKey);
                    _gizmoGuiSkin = AssetDatabase.LoadAssetAtPath(path, typeof (GUISkin)) as GUISkin;
                }
                return _gizmoGuiSkin;
            }
#endif
            return null;
        }
    }

    //private static GUISkin LocalGizmoGuiSkin
    //{
    //    get
    //    {
    //        if (EditorPrefs.HasKey("GizmoGuiSkin"))
    //        {
    //            if (_loadedGizmoGuiSkin != _gizmoGuiSkin || _loadedGizmoGuiSkin==null)
    //            {
    //                var path = EditorPrefs.GetString("GizmoGuiSkin");
    //                _loadedGizmoGuiSkin = AssetDatabase.LoadAssetAtPath(path, typeof (GUISkin)) as GUISkin;
    //            }
    //            return _loadedGizmoGuiSkin;
    //        }
    //        return null;
    //    }
    //}
#if UNITY_EDITOR
    [MenuItem("Window/CustomSettings")]
    private static void Init()
    {
        CustomEditorPrefs win = GetWindow<CustomEditorPrefs>();
        _gizmoGuiSkin = GizmoGuiSkin;
        win.Show();
    }

    private static GUISkin _gizmoGuiSkin;
    //private static GUISkin _loadedGizmoGuiSkin;

    private void OnGUI()
    {
        var skin = EditorGUILayout.ObjectField(_gizmoGuiSkin, typeof (GUISkin), false);

        if (skin != null)
        {
            _gizmoGuiSkin = (GUISkin) skin;


            if (GizmoGuiSkin != _gizmoGuiSkin)
            {
                EditorPrefs.SetString(GizmoGuiSkinKey, AssetDatabase.GetAssetPath(_gizmoGuiSkin));
            }
        }

        if (GUILayout.Button("Save"))
        {
            EditorPrefs.SetString(GizmoGuiSkinKey, AssetDatabase.GetAssetPath(_gizmoGuiSkin));
        }
    }
#endif
}
