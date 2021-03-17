using UnityEngine;
using System.Collections;

public class ResizeLabelWidth : MonoBehaviour 
{
    private void Start()
    {
        Resize();
        //Invoke("Resize",3);
    }


    public void Resize()
    {
        var lbl = GetComponent<UILabel>(); //label.UpdateNGUIText() must be called before any NGUIText functions.
        lbl.UpdateNGUIText();
        float width = NGUIText.CalculatePrintedSize(lbl.text).x;// *lbl.fontSize;
        lbl.width = (int)width;
    }

}
