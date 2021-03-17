using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BtnApplicationQuit : MonoBehaviour
{
    [SerializeField]
    private float _quitTime = 0.3f;

    private void Start()
    {
    }

    private void OnClick()
    {
        Invoke("Quit", _quitTime);
    }

    private void Quit()
    {
        Application.Quit();
    }

}
