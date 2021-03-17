using UnityEngine;
using System.Collections;

public class GuiScreenRecordLineFix : MonoBehaviour
{
    private UIRect _uiItem;

	void Start ()
	{
        _uiItem = this.GetSafeComponent<UIRect>();

        //float w = Screen.width ;
        //float h = Screen.height;

	    //float res = h/w;
        //Debug.LogWarning(res);

        _uiItem.leftAnchor.target = _uiItem.rightAnchor.target = transform.root.Find("Panel1");

	    _uiItem.leftAnchor.Set(0f, 0f);
	    _uiItem.rightAnchor.Set(1f, 0f);

        _uiItem.ResetAnchors();
	}


}
