using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BtnSetControlActive : MonoBehaviour
{
    [SerializeField]
    private GameObject _menu;

    [SerializeField]
    private bool _activeState;

    private void Start()
    {
    }

    private void OnClick()
    {
        NGUITools.SetActive(_menu, _activeState);
    }
}
