using UnityEngine;
using System.Collections;

public class GuiScreenTutorialLogoIconFix : MonoBehaviour
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
            _uiItem.leftAnchor.Set(0f, 32f);
            _uiItem.rightAnchor.Set(0f, 284f);
            _uiItem.bottomAnchor.Set(0.5f, -126f);
            _uiItem.topAnchor.Set(0.5f, 126f);
	    }

        //228
        if (res > 1.45 && res < 1.55) //3:2
        {
            _uiItem.leftAnchor.Set(0f, 32f);
            _uiItem.rightAnchor.Set(0f, 260f);
            _uiItem.bottomAnchor.Set(0.5f, -114f);
            _uiItem.topAnchor.Set(0.5f, 114f);
        }

        //не нужно
        //if (res < 1.45) //4:3
	}

   
}
