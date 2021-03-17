using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class HideSelfOnGameEnd : MonoBehaviour
{
	void Start () 
	{
        EventAggregator.Subscribe(GameEvent.EngGameProcess, this, Hide);
	}
	
	void Hide ()
	{
	    gameObject.SetActive(false);
	}
}
