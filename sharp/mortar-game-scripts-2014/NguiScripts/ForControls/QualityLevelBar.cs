using System.Linq;
using UnityEngine;
using System.Collections;

public class QualityLevelBar : MonoBehaviour 
{
    private UIToggle[] _toggles;

    void Awake()
    {
        _toggles = this.GetComponentsInDirectChildrens<UIToggle>().OrderBy(c => c.gameObject.name).ToArray();
    }

    void OnEnable()
    {
        var level = QualitySettings.GetQualityLevel();
        _toggles[level].value = true;
    }
}
