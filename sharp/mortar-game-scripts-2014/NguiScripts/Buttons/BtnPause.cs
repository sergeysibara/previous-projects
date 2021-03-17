using UnityEngine;
using System.Collections;

public class BtnPause : MonoBehaviour
{
    [SerializeField]
    private bool _pauseStateOnClick;

    private void Start()
    {
    }

    private void OnClick()
    {
        //Debug.LogWarning("click btn");
        BattleManager.Instance.Pause = _pauseStateOnClick;
    }
}
