using System;
using System.Collections;
using UnityEngine;


// Должен быть только в title сцене, т.к. не удаляется
public class SoundManager : MonoSingleton<SoundManager>
{
    /// <summary>
    /// AudioSource в сценах, кроме сцен боя. 
    /// </summary>
    private AudioSource _state1Audio;

    /// <summary>
    /// AudioSource для сцен боя
    /// </summary>
    private AudioSource _state2Audio;

    [SerializeField]
    private AudioClip[] _clipsForGameLevels;

    [SerializeField]
    private AudioClip _titleClip;

    [SerializeField,Tooltip("Время между окончанием первого клипа и стартом проигрывания второго")]
    private float _nextClipStartTime = 1f;

    private AudioSource _nextAudio = null;

    [SerializeField]
    private float _startVolume = 0.05f;

    private void Start()
    {
        var audios = GetComponents<AudioSource>();
        _state1Audio = audios[0];
        _state2Audio = audios[1];

        _state1Audio.clip = _titleClip;

        if (IsState1)
            _state1Audio.Play();
        else
            _state2Audio.Play();

    }

    private void Update()
    {
        if ((IsState1 && _state1Audio.isPlaying) || (!IsState1 && _state2Audio.isPlaying) || _nextAudio!=null)
            return;

        if (!_state1Audio.isPlaying && !_state2Audio.isPlaying)
        {
            _nextAudio = IsState1 ? _state1Audio : _state2Audio;
            Invoke("StartAudio", _nextClipStartTime);
            return;
        }

        if (IsState1)
        {
            _nextAudio = _state1Audio;
            Invoke("StartAudio", _nextClipStartTime);
            StartCoroutine(FadeOutCoroutine(_state2Audio));
        }
        else
        {
            _state2Audio.clip = RandomUtils.GetRandomItem(_clipsForGameLevels);
            _nextAudio = _state2Audio;
            Invoke("StartAudio", _nextClipStartTime);
            StartCoroutine(FadeOutCoroutine(_state1Audio));
        }
    }

    private void StartAudio()
    {
        if (!IsState1)
            _state2Audio.clip = RandomUtils.GetRandomItem(_clipsForGameLevels);
        _nextAudio.Play();
        _nextAudio = null;
    }


    private IEnumerator FadeOutCoroutine(AudioSource target)
    {
        while (target.volume > 0)
        {
            target.volume -= 0.01f;
            yield return new WaitForSeconds(0.1f);
        }

        target.Stop();
        target.volume = _startVolume;
    }

    /// <summary>
    /// при = true - Не сцена боя
    /// </summary>
    private bool IsState1
    {
        get { return !Conditions.Application.IsBattleScene; }
    }

    ///// <summary>
    ///// Включает или отключает звук
    ///// </summary>    
    //public static void SetSound(bool enable)
    //{
    //    AudioListener.volume = enable ? 1 : 0;
    //    //Debug.LogWarning(enable);
    //}
}

