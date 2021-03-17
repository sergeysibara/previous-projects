using UnityEngine;
using System.Collections;

public class ButtonInvokeAdjuster : MonoBehaviour
{
    private void Start()
    {
    }

    private void OnClick()
    {
        if (MainSceneManager.CurrentGameMode == MainSceneManager.GameMode.Normal)
        {
            EventMessenger.SendMessage(GameEvent.InvokeAdjuster, this);
            MainSceneManager.CurrentGameMode = MainSceneManager.GameMode.InvokeAdjuster;
        }        
    }
}
