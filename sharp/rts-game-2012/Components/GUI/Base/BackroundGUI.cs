using UnityEngine;

public class BackroundGUI
{
    public Rect RightPanelGroup;
    public Rect RightPanel;
    public Rect StatsPanel;
    public Rect StatsPlayerMoney;
    public Rect StatsCursorState;
    public string RightPanelTitle;

    //поля для вывода дополнительной инфы (например, для дебага запущенного приложения)
    public Rect StatsAdditionalRect;
    public string StatsAdditionalText;

    public BackroundGUI()
    {
        RightPanelTitle = "Buying menu"; 
        RightPanelGroup = new Rect(Screen.width - 185.0f, 0.0f, 185.0f, Screen.height - 300.0f); 
        RightPanel = new Rect(0, 0.0f, 185.0f, Screen.height - 300.0f); 
        
        StatsPanel = new Rect(0.0f, 0.0f, 150.0f, 100.0f);
        StatsPlayerMoney = new Rect(StatsPanel.x + 12.5f, StatsPanel.y + 30.0f, 125.0f, 25.0f);
        StatsCursorState = new Rect(StatsPanel.x + 12.5f, StatsPanel.y + 50.0f, 125.0f, 50.0f);

        StatsAdditionalRect = new Rect(StatsPanel.x + 12.5f, StatsPanel.y + 100.0f, 125.0f, 100.0f);
    }

    public void UpdatePanelsPosition()
    {
        RightPanelGroup.x = Screen.width - 185.0f;
        RightPanelGroup.height = Screen.height - 300.0f;
        RightPanel.height=Screen.height - 300.0f;
    }
}
