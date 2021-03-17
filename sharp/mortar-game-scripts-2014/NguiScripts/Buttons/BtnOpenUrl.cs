using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BtnOpenUrl : MonoBehaviour
{
    [SerializeField]
    private string _url = "http://www.domru.ru";

    private void Start()
    {
    }

    private void OnClick()
    {
        Application.OpenURL(_url);
    }
}
