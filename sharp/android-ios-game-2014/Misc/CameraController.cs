using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
    void Start()
	{
	    camera.enabled = false;
        EventMessenger.Subscribe(GameEvent.StartGameProcess, this, ()=>camera.enabled = true);
	}
}
