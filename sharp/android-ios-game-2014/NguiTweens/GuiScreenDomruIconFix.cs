using UnityEngine;
using System.Collections;

public class GuiScreenDomruIconFix : MonoBehaviour
{
    private UIRect _uiItem;

    void Awake()
	{
        _uiItem = this.GetSafeComponent<UIRect>();

        float w = Screen.width ;
        float h = Screen.height;

	    float res = h/w;
        //Debug.LogWarning(res);

	    if (res > 1.55) //16:10
	    {
            _uiItem.leftAnchor.Set(1f, -277f);
            _uiItem.rightAnchor.Set(1f, -25f);
            _uiItem.bottomAnchor.Set(0.5f, -126f);
            _uiItem.topAnchor.Set(0.5f, 126f);
	    }

        //228
        if (res > 1.45 && res < 1.55) //3:2
        {
            _uiItem.leftAnchor.Set(1f, -253f);
            _uiItem.rightAnchor.Set(1f, -25f);
            _uiItem.bottomAnchor.Set(0.5f, -114f);
            _uiItem.topAnchor.Set(0.5f, 114f);
        }

        //не нужно
        //if (res < 1.45) //4:3
	}

   
}
