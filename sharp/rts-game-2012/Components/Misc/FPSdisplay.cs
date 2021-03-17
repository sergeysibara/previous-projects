using UnityEngine;

public class FPSdisplay : MonoBehaviourHeritor
{
	float updatePeriod = 0.5f;
	float nextUpdate  = 0;
	float frames  = 0;
	float fps = 0;
    Rect fpsInfoRect=new Rect(150,0,200,30);

	void Update () 
	{
		frames++;
		if (Time.time > nextUpdate)
		{
			fps = Mathf.Round(frames / updatePeriod);
			nextUpdate = Time.time + updatePeriod;
			frames = 0;
		}
	}

    void OnGUI()
    {
        GUI.Label(fpsInfoRect, "FPS: "+fps);
    }
}