using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public abstract class BaseGUI : MonoBehaviourHeritor 
{
    [SerializeField]
    protected int _guiDepth = 0;


    [HideInInspector]
    public bool MouseOverGUI;
    public BackroundGUI GUIBackLayer=new BackroundGUI();

    protected virtual void Update()
    {
        GUIBackLayer.UpdatePanelsPosition();
    }

    protected virtual void OnGUI()
    {
        GUI.depth = _guiDepth;
        ShowGUIBackLayer();
        CheckIsMouseOverGUI();
    }

    protected void ShowGUIBackLayer()
    {
        GUI.BeginGroup(GUIBackLayer.RightPanelGroup);
        GUI.Box(GUIBackLayer.RightPanel, GUIBackLayer.RightPanelTitle);
        GUI.EndGroup();

        GUI.Box(GUIBackLayer.StatsPanel, "Player Stats");
        GUI.Label(GUIBackLayer.StatsPlayerMoney, "Money: " + GameManager.CurrentPlayer.Money + "$");
        
        #if UNITY_EDITOR
            GUI.Label(GUIBackLayer.StatsCursorState, "Cursor state: " + GameManager.CurrentPlayer.CursorState);
        #endif

        GUI.Label(GUIBackLayer.StatsAdditionalRect, GUIBackLayer.StatsAdditionalText);
    }

    protected void CheckIsMouseOverGUI()
    {
        MouseOverGUI = false;

        //проверка каждой панели. ѕровер€ютс€ только панели в GUIBackLayer
        MouseOverGUI = GUIBackLayer.RightPanelGroup.Contains(Event.current.mousePosition);
        if (!MouseOverGUI)
            MouseOverGUI = GUIBackLayer.StatsPanel.Contains(Event.current.mousePosition);
    }

}
