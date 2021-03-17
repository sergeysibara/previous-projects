using UnityEngine;
using System.Collections;

public class BtnGetBonus : MonoBehaviour 
{
	void Start () 
	{
	
	}

    private void OnClick()
    {
        GlobalVariables.AdditionalExplosionLevel = 3;
    }
}
