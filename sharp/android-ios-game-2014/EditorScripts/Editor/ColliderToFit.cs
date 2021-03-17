using UnityEngine;
using UnityEditor;
using System.Collections;

public class ColliderToFit
{

    [MenuItem("Tools/Collider/Fit BoxCollider to Children")]
    static void FitToChildren()
    {
        foreach (GameObject rootGameObject in Selection.gameObjects)
        {
            if (rootGameObject.collider == null)
                rootGameObject.AddComponent<BoxCollider>();
            else if (!(rootGameObject.collider is BoxCollider))
                continue;

            bool hasBounds = false;
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

            var renderers = rootGameObject.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (hasBounds)
                {
                    bounds.Encapsulate(renderer.bounds);
                }
                else
                {
                    bounds = renderer.bounds;
                    hasBounds = true;

                }
            }

            BoxCollider collider = (BoxCollider)rootGameObject.collider;
            collider.center = bounds.center - rootGameObject.transform.position;
            collider.size = bounds.size;
        }
    }

}