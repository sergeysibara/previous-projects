using UnityEngine;
using System.Collections;

public class BrigthSliderBinding : MonoBehaviour 
{
    private UISlider _slider;

    // Use this for initialization
    void Start()
    {
        _slider = gameObject.GetComponent<UISlider>();

        //Debug.LogWarning(SettingsManager.Instance.Brigth);
        _slider.value = SettingsManager.Instance.Brigth;
        _slider.onChange.Add(new EventDelegate(SettingsManager.Instance.SetBright));
    }

    //void OnValueChange()
    //{
    //    AudioListener.volume = val;
    //    //    print(val);
    //}
}
