using UnityEngine;

public static class GUIManager
{
    public static void DrawBuyButton(Rect rect, GameObject original, string text, int price, HumanPlayer player)
    {
        if (original != null)
        {
            if (GUI.Button(rect, string.Format("{0}\n{1}$", text, price)))
            {
                GameManager.CurrentPlayer.ObjectPlacer.CanselPlacing();

                if (player.Money - price >= 0)
                {
                    BuildingGrid grid;
                    GameObjectManager.CreateGhost(original, out grid);
                    player.ObjectPlacer.StartPlacing(grid, price);
                }
            }
        }
        else
            if (GUI.Button(rect, string.Format("{0}\n{1}$", text, 0)))
            {
                GameManager.CurrentPlayer.ObjectPlacer.CanselPlacing();
            }
    }

    public static void DrawCommandButton(Rect rect, CommandButton commandButton)
    {
        if (GUI.Button(rect, commandButton.GetText()))
        {
            commandButton.OnHumanPlayerClick();
        }
    }
}
