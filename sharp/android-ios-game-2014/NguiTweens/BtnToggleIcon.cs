using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BtnToggleIcon : MonoBehaviour
{
    [SerializeField]
    private string _state1;

    [SerializeField]
    private string _state2;

    public string StateOn {
        get { return _state1; }
    }

    public string StateOff
    {
        get { return _state2; }
    }

    private void Start()
    {
    }

    private void OnClick()
    {
        var btn = GetComponent<UIButton>();

        if (btn.normalSprite == _state1)
            btn.normalSprite = _state2;
        else if (btn.normalSprite == _state2)
            btn.normalSprite = _state1;
        else
            Debug.LogError("undefined sprite");
    }
}
