using UnityEngine;
using System.Collections;

public class BtnSaveSettings : MonoBehaviour 
{
	void Start () 
	{
	
	}

    private void OnClick()
    {
        SettingsManager.Instance.Save();
    }
}
