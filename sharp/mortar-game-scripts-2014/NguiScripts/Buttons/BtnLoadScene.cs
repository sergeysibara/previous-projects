using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BtnLoadScene : MonoBehaviour
{
    [SerializeField]
    private Consts.SceneNames _sceneName;

    private void Start()
    {
    }

    private void OnClick()
    {
        Application.LoadLevel(_sceneName.GetStringValue<StringValueAttribute>()); 
    }
}
