using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//основа взята из: http://www.unity3d.ru/distribution/viewtopic.php?f=5&t=8737

//Необходимо добавить очередь для передаваемых данных и при окончании выполнения текущей корутины извлекать их, 
//иначе есть риск выполнения нескольких корутин в одно время.
public class RuntimeBatching : MonoSingleton<RuntimeBatching>
{
    public static void DoStaticBatching(Transform obj, GameObject staticBatchRoot = null)
    {
        Current.StartCoroutine(Current.StaticBatchingCoroutine(obj, staticBatchRoot));
    }

    IEnumerator StaticBatchingCoroutine(Transform obj, GameObject staticBatchRoot)
    {
        string batchObjName = obj.name;
        
        //Передаваемый объект НЕ должен присутствовать в объекте parentContainer !!!
        Transform parentContainer;
        if (obj.IsBuilding())
            parentContainer=GameObjectContainers.StaticBuildings;
        else
            parentContainer=GameObjectContainers.StaticMisc;


        var meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();//список всех MeshRenderers у батчимого объекта. Будут батчиться только те объекты, у которых есть MeshRenderer
        
        List<GameObject>[] goLists = new List<GameObject>[meshRenderers.Length];//массив списков всех батчимых объектов
        for (int i = 0; i < goLists.Length; i++)
            goLists[i] = new List<GameObject>();

        Mesh[] sharedMeshes = new Mesh[goLists.Length];
        for (int i = 0; i < goLists.Length; i++)
        {
            var meshFilter = meshRenderers[i].GetComponent<MeshFilter>();
            if (meshFilter != null)
                sharedMeshes[i] = meshFilter.sharedMesh;
        }

        //Поиск (по имени) объектов на сцене того же типа, что и батчимый объект. 
        foreach (Transform tr in parentContainer)
        {
            if (tr.gameObject.active && tr.name == batchObjName)
            {
                var childRenderers = tr.GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < goLists.Length; i++) //Цикл заполнения массивов для батчинга и обновление мешей
                {
                    var childObj = childRenderers[i].gameObject;
                    goLists[i].Add(childObj);
                    childObj.isStatic = true;

                    MeshFilter meshFilter = childObj.GetComponent<MeshFilter>();
                    Renderer meshRenderer = childRenderers[i];
                    Material[] materials = meshRenderer.sharedMaterials;

                    DestroyImmediate(meshRenderer);

                    if (meshFilter != null)
                    {
                        if (sharedMeshes[i] != null)
                            meshFilter.sharedMesh = sharedMeshes[i];
                        meshRenderer = childObj.AddComponent<MeshRenderer>();
                        //meshRenderer.castShadows = false;
                        //meshRenderer.receiveShadows = false;
                        meshRenderer.sharedMaterials = materials; //meshRenderers[i].sharedMaterials;
                    }
                }
            }
        }

        //добавляем сам объект в массив батчимых объектов
        obj.parent = parentContainer;
        obj.gameObject.isStatic = true;
        for (int i = 0; i < goLists.Length; i++)
        {
            //meshRenderers[i].castShadows = false;
            //meshRenderers[i].receiveShadows = false;
            goLists[i].Add(meshRenderers[i].gameObject);
        }

        for (int i = 0; i < goLists.Length; i++)
            StaticBatchingUtility.Combine(goLists[i].ToArray(), staticBatchRoot);

        yield return Resources.UnloadUnusedAssets();
    }

}
