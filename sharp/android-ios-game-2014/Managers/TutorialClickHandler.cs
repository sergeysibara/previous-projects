using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Обрабатывает только клики по установленной цели
/// </summary>
public class TutorialClickHandler : RequiredMonoSingleton<TutorialClickHandler>
{
    [SerializeField]
    private bool _isAcrive = true;

    [SerializeField]
    private Transform _target;

    private LayerMask _clickableMmask = int.MaxValue;
    
    private void Start()
    {
        _clickableMmask = _clickableMmask.RemoveFromMask(Consts.Layers.Signals);//, Consts.Layers.Obstacles);
    }

    //public static bool MouseOverGUI = false;

    private void Update()
    {
        if (!_isAcrive)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _clickableMmask))
            {
                if (hit.transform == _target)
                {
                    hit.transform.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
                    EventMessenger.SendMessage(GameEvent.OnTutorial_ClickByTarget, this);
                    EventMessenger.SendMessage(GameEvent.EngGameProcess, this);
                    _isAcrive = false;
                }
            }
        }
    }
}
