using UnityEngine;
using System.Collections;

public class UIPanelFix : MonoBehaviour
{
    private UIPanel panel;

    private void Awake()
    {
        panel = GetComponent<UIPanel>();
        panel.alpha = 0f;
    }

    private void Start()
    {
        if (Application.loadedLevelName != Consts.SceneNames.Records.ToString())
            EventMessenger.Subscribe(GameEvent.StartGameProcess, this, Show);
        else
        {
            EventMessenger.Subscribe(GameEvent.OnFillRecordsTable, this, Show);
            Invoke("Show", 0.3f);
        }
    }

    private void Show()
    {
        panel.alpha = 1f;
    }
	
}
