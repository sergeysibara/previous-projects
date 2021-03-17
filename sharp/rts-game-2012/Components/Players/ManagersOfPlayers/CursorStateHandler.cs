using UnityEngine;
using System.Collections.Generic;

public class CursorStateHandler : MonoBehaviourHeritor 
{
    HumanPlayer _player;
    
    protected override void Awake()
    {
        base.Awake();
        _player = GetComponent<HumanPlayer>();
    }

    void Update()
    {
        bool mouseOverGUI = (_player.ActiveGUI != null && _player.ActiveGUI.MouseOverGUI);

        switch (_player.CursorState)
        {
            case CursorStates.Default:
                {
                    if (!mouseOverGUI)
                    {
                        if (_player.ObjectSelector.SelectedObjectList.Count>0 && Input.GetMouseButtonDown(1))
                        {
                            _player.CommandSender.AutoDeterminedPointCommand(Input.mousePosition);
                            break;
                        }

                        if (Input.GetMouseButtonDown(0))
                        {
                            _player.ObjectSelector.StartSelecting(Input.mousePosition);
                        }
                    }
                } 
                break;

            case CursorStates.Selecting:
                {
                    _player.ObjectSelector.OnLMB_Pressing(Input.mousePosition);

                    if (Input.GetMouseButtonUp(0) || Input.GetMouseButton(1))
                        _player.ObjectSelector.Select();
                } 
                break;

            case CursorStates.Placing:
                {
                    if (!mouseOverGUI)
                    {
                        if (Input.GetMouseButtonDown(1))
                        {
                            _player.ObjectPlacer.CanselPlacing();
                            break;
                        }

                        Ray scrRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(scrRay, out hit, Mathf.Infinity, GameManager.GroundLayers))
                        {
                            _player.ObjectPlacer.MoveObjectToNodeCenter(hit.point);
                            
                            //проверка доступности размещения объекта и изменение его цвета
                            bool placementIsAvailable = _player.ObjectPlacer.PlacementIsAvailable();

                            if (Input.GetMouseButtonDown(0))
                            {
                                if (placementIsAvailable)
                                    _player.ObjectPlacer.Place();
                            }
                        }
                    }
                }
                break;
        }
    }

}
