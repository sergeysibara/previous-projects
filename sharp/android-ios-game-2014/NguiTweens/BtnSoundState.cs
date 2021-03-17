using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BtnSoundState : MonoBehaviour
{
    private BtnToggleIcon _btnToggleIcon;

    private UIButton _btn;


    private void Start()
    {
        _btnToggleIcon = GetComponent<BtnToggleIcon>();
        _btn = GetComponent<UIButton>();

        int disableSound = PlayerPrefs.GetInt("DisableSound");//при 0 - звук есть, при 1 -нету
        if (disableSound == 1)
        {
            _btn.normalSprite = _btnToggleIcon.StateOff;
        }
    }

    private void OnClick()
    {
        int disableSound = PlayerPrefs.GetInt("DisableSound");//при 0 - звука есть, при 1 -нету
        if (disableSound == 0)
        {
            //отключить звук
            SoundManager.SetSound(false);
            PlayerPrefs.SetInt("DisableSound", 1);
        }
        else
        {
            //включить звук
            SoundManager.SetSound(true);
            PlayerPrefs.SetInt("DisableSound", 0);
        }
    }
}
