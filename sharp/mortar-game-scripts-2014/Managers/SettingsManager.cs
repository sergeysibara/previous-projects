using UnityEngine;
using System.Collections;

public class SettingsManager : MonoSingleton<SettingsManager>
{
    //[Range(0, 1)][Tooltip("Настройка яркости, работает только в игре")]
    //public float BrigthValue;

    /// <summary>
    /// Коэфициент, во сколько реальная яркость отличается от задаваемой ползунком.
    /// </summary>
    [SerializeField]
    private float _brightFactor = 0.5f;


    private readonly Color _defaultAmbientLight = new Color(127, 127, 127, 0);

    private float _hue;
    private float _saturation;
    private float _brigth;


    private float _volume;
    private int _qualityLevel;

    private const string _brigthKey = "bright";
    private const string _volumeKey = "volume";
    private const string _qualityLevelKey = "qualityLevel";

    protected override void Awake()
    {
        base.Awake();
        LoadSetting();
    }

    private void OnLevelWasLoaded(int level)
    {
        LoadLevelBrigth();
    }

    private void LoadSetting()
    {
        LoadLevelBrigth();

        //volume load
        if (PlayerPrefs.HasKey(_volumeKey))
        {
            _volume = PlayerPrefs.GetFloat(_volumeKey);
            AudioListener.volume = _volume;
        }
        else
            _volume = AudioListener.volume;

        //Debug.LogWarning("AudioListener.volume=" + AudioListener.volume);


        //_qualityLevelKey load
        if (PlayerPrefs.HasKey(_qualityLevelKey))
        {
            _qualityLevel = PlayerPrefs.GetInt(_qualityLevelKey);
            QualitySettings.SetQualityLevel(_qualityLevel);
            //Debug.LogWarning("_qualityLevel="+_qualityLevel);
        }
        else
            _qualityLevel = QualitySettings.GetQualityLevel();
    }

    /// <summary>
    /// В каждой сцене свой цвет BattleAmbientLight, поэтому загрузка отдельно в каждом уровне
    /// </summary>
    private void LoadLevelBrigth()
    {
        if (Conditions.Application.IsBattleScene)
        {
            ColorUtils.ColorToHSV(BattleManager.Instance.BattleAmbientLight, out _hue, out _saturation, out _brigth);

            if (PlayerPrefs.HasKey(_brigthKey))
                _brigth = PlayerPrefs.GetFloat(_brigthKey);

            RenderSettings.ambientLight = ColorUtils.ColorFromHSV(_hue, _saturation, _brigth);
        }
        else
        {
            RenderSettings.ambientLight = _defaultAmbientLight;
        }
        RenderSettings.ambientLight = ColorUtils.ColorFromHSV(_hue, _saturation, _brigth);
    }

    public float Brigth
    {
        get { return _brigth / _brightFactor; }
        set
        {
            _brigth = Mathf.Clamp01(value * _brightFactor);
            RenderSettings.ambientLight = ColorUtils.ColorFromHSV(_hue, _saturation, _brigth);
        }
    }

    public void SetBright()
    {
        if (UIProgressBar.current != null)
            Brigth = UIProgressBar.current.value;
    }



    public void SetVolume()
	{
		if (UIProgressBar.current != null)
            Volume = UIProgressBar.current.value;
    }

    public float Volume
    {
        get { return _volume; }
        set
        {
            _volume = Mathf.Clamp01(value);
            AudioListener.volume = _volume;
        }
    }

    public int QualityLevel
    {
        get { return _qualityLevel; }
        set
        {
            _qualityLevel = Mathf.Clamp(value, 0, 2);
            QualitySettings.SetQualityLevel(_qualityLevel);
        }
    }


    public void Save()
    {
        PlayerPrefs.SetFloat(_brigthKey, _brigth);
        PlayerPrefs.SetFloat(_volumeKey, _volume);
        PlayerPrefs.SetInt(_qualityLevelKey, _qualityLevel);

        PlayerPrefs.Save();
    }
}

public enum QualityLevel
{
    Low=0,
    Normal=1,
    High=2,
}