using UnityEngine;
using System.Collections;


public class BuyingMenu : BaseGUI
{
    public GameObject Tower;
    public GameObject Barrack;

    const int _maxButtonsCount = 9;
    Rect[] _buttonRects = new Rect[_maxButtonsCount];
    Rect _tower; 
    Rect _barrack;
    Rect _warrior;
    //Rect _wall;
    //Rect _gate;

    protected override void Awake()
    {
        base.Awake();

        float left = 12.5f;
        float top = - 30f;
        float width = 160f;
        float height = 50f;
        float yInterval = 60f;

        for (int i = 0; i < _maxButtonsCount; i++)
            _buttonRects[i] = new Rect(left, top += yInterval, width, height);
    }

    protected override void OnGUI()
    {
        base.OnGUI();

        GUI.BeginGroup(GUIBackLayer.RightPanelGroup);
            if (GameManager.CurrentPlayer.GUIPreProcessor.CommandButtonList.Count > 0)
            {
                for (int i = 0; i < GameManager.CurrentPlayer.GUIPreProcessor.CommandButtonList.Count && i < _maxButtonsCount; i++)
                    GUIManager.DrawCommandButton(_buttonRects[i], GameManager.CurrentPlayer.GUIPreProcessor.CommandButtonList[i]);   
            }
            else
            {
                //кнопки по умолчанию
                GUIManager.DrawBuyButton(_buttonRects[0], Tower, "Tower", CommonPrice.Tower, GameManager.CurrentPlayer);
                GUIManager.DrawBuyButton(_buttonRects[1], Barrack, "Barrack", CommonPrice.Barrack, GameManager.CurrentPlayer);
                //GUIManager.DrawBuyButton(_buttonRects[3], null, "Wall", 0, GameManager.CurrentPlayer);
                //GUIManager.DrawBuyButton(_buttonRects[4], null, "Gate", 0, GameManager.CurrentPlayer);
            }
        GUI.EndGroup();      
    }

}
