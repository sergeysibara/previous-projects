using UnityEngine;
using System.Collections;

public class GlobalGameManager : RequiredMonoSingleton<GlobalGameManager>
{

    void Start () 
    {
	    DontDestroyOnLoad(gameObject);
	}
}
