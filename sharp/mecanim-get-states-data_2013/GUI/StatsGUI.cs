using UnityEngine;

public class StatsGUI : MonoBehaviour
{
    [HideInInspector]
    public string Title;

    [HideInInspector]
    public string Text;

    private Rect _statsPanel = new Rect(0.0f, 0.0f, 300.0f, 1000.0f);

    protected virtual void OnGUI()
    {
        GUILayout.BeginArea(_statsPanel);
        GUILayout.BeginVertical("box");
        {
            GUILayout.Label(Title);
            GUILayout.Label(Text);
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

}
