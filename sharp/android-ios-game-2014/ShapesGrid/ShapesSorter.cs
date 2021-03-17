using System.Collections.Generic;

using UnityEngine;
using System.Collections;


public class ShapesSorter : RequiredMonoSingleton<ShapesSorter>
{
    [SerializeField]
    private List<ChainItem> _chainItems;

    private void Start()
    {
        EventMessenger.Subscribe(GameEvent.InvokeAdjuster, this, StartSorting);
	}

    public static void SetChain(List<ChainItem> chainItems)
    {
        Instance._chainItems = chainItems;
    }

    private void StartSorting()
    {
        StartCoroutine(Instance.SortChainRecursively(Instance._chainItems));
    }

    private IEnumerator SortChainRecursively(List<ChainItem> chainItems)//string name, int level)
    {
        foreach (var item in chainItems)
        {
            if (item.Shape != null)
            {
                item.Shape.RotateToDirection(item.TargetDirection);
                //Debug.LogWarning(item.Shape.name, item.Shape);
                yield return new WaitForSeconds(0.05f);

                if (item.childChain != null)
                    StartCoroutine(SortChainRecursively(item.childChain));
            }
        }
    }

}
