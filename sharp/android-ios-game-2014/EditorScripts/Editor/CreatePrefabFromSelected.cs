using UnityEditor;
using UnityEngine;

/// <summary>
/// Creates a prefab from a selected game object.
/// </summary>
class CreatePrefabFromSelected
{
    const string menuName = "Assets/Custom/Create Prefab From Selected";

    /// <summary>
    /// Adds a menu named "Create Prefab From Selected" to the GameObject menu.
    /// </summary>
    [MenuItem(menuName)]
    static void CreatePrefabMenu()
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            //var go = Selection.activeGameObject;
            //var prefab = PrefabUtility.CreateEmptyPrefab("Assets/" + go.name + ".prefab");

            var prefab = AssetDatabase.LoadAssetAtPath("Assets/" + go.name + ".prefab", typeof(GameObject));
            if (prefab==null)
                prefab = PrefabUtility.CreateEmptyPrefab("Assets/" + go.name + ".prefab");

            PrefabUtility.ReplacePrefab(go, prefab, ReplacePrefabOptions.ReplaceNameBased);
        }
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// Validates the menu.
    /// The item will be disabled if no game object is selected.
    /// </summary>
    /// <returns>True if the menu item is valid.</returns>
    [MenuItem(menuName, true)]
    static bool ValidateCreatePrefabMenu()
    {
        return Selection.activeGameObject != null;
    }
}