using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ButtonOpenMenu : MonoBehaviour
{
    public GameObject _openMenu;

    private void Start()
    {
    }

    private void OnClick()
    {
        //находим все объекты с тегом 'Menu'
        var rootChilds = transform.root.GetComponentsInChildren<Transform>();
        IList<GameObject> menus = new List<GameObject>();
        foreach (var rootChild in rootChilds)
        {
            if (rootChild.CompareTag(Consts.Tags.Menu))
                menus.Add(rootChild.gameObject);
        }

        //прячем каждый объект
        foreach (GameObject go in menus)
            NGUITools.SetActive(go, false);
        //показываем объект '_openMenu'
        NGUITools.SetActive(_openMenu, true);
    }
}
