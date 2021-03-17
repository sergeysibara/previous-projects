using UnityEngine;
using System.Collections;

public class Exit : MonoBehaviourHeritor 
{
    void Update() 
    {
        if (Input.GetKey("escape"))
            Application.Quit();
    }
}
