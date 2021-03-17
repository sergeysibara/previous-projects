using UnityEngine;
using System.Collections;

public class Device : MonoBehaviour
{
    [SerializeField]
    private bool _playAudioOnSwitchToON = true;


    private float timeForOFF;
    private SpriteChanger _spriteChanger;

    private RandomPlayAudio _randomPlayAudio;

    private void Start()
    {
        _spriteChanger = GetComponent<SpriteChanger>();
        _randomPlayAudio = GetComponent<RandomPlayAudio>();    
        enabled = false;
    }

    public void SwitchToOn()
    {
        if (enabled) 
            return;

        enabled = true;
        if (_spriteChanger != null)
            _spriteChanger.SwitchToOn();//.enabled = true;

        if (!_playAudioOnSwitchToON)
            return;

        if (_randomPlayAudio!=null)
            _randomPlayAudio.Play();
        else if (audio!=null)
            audio.Play();
    }

    public void SwitchToOff()
    {
        if (!enabled)
            return;

        enabled = false;
        if (_spriteChanger != null)
            _spriteChanger.SwitchToOff();//.enabled = false;

        if (!_playAudioOnSwitchToON)
            return;

        if (_randomPlayAudio != null)
            _randomPlayAudio.Stop();
        else if (audio != null)
            audio.Stop();
    }

}
