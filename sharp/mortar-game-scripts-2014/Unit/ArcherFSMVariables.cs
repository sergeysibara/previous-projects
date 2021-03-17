using UnityEngine;
using System.Collections;

public class ArcherFSMVariables : MonoBehaviour
{ 
    /// <summary>
    /// Цель, чтобы юнит остановился или двигался к нужной точке, а не чекпоинту.
    /// </summary>
    [HideInInspector]
    public Transform SecondTarget;

	void Awake () 
	{
        SecondTarget = transform.FindChild("SecondTarget");
        SecondTarget.parent = SceneContainers.Targets;
	}
	
	void Update () 
	{
	
	}
}
