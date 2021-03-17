using UnityEngine;

public class GuiScreenFixCamera : MonoBehaviour
{
    void Awake()
	{
        float w = Screen.width ;
        float h = Screen.height;
	    float aspect = h/w;

        const float startHeight=12.9933f;//высота камера, при aspect ratio = 0
        const float increment=6.0733f;//прирост высоты при увеличении aspect ratio на 1.

        float newYpos = startHeight + aspect*increment;
        camera.transform.SetY(newYpos);
	}
}
