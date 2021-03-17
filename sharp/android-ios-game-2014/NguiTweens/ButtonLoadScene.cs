using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ButtonLoadScene : MonoBehaviour
{
    [SerializeField]
    private Consts.SceneNames _sceneName;

    private void Start()
    {
    }

    private void OnClick()
    {
        Application.LoadLevel(_sceneName.ToString()); 
    }
}
