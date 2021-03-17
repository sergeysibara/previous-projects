using UnityEngine;
using System.Collections;

public class BtnQualityLevel : MonoBehaviour
{
    [SerializeField]
    private QualityLevel _qualityLevel;

	void Start () 
	{
	
	}


    private void OnClick()
    {
        SettingsManager.Instance.QualityLevel = (int) _qualityLevel;
    }
}
