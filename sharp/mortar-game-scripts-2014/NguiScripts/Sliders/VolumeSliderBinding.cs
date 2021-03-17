using UnityEngine;
using System.Collections;

public class VolumeSliderBinding : MonoBehaviour 
{
    private UISlider _slider;

    void Start()
    {
        _slider = gameObject.GetComponent<UISlider>();

        //Debug.LogWarning(SettingsManager.Instance.Volume);
        _slider.value = SettingsManager.Instance.Volume;
        _slider.onChange.Add(new EventDelegate(SettingsManager.Instance.SetVolume));
    }
}
