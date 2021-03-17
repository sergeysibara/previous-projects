using UnityEngine;
using System.Collections;

public class PlayAudioOnClick : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] _clips;

    private void Start()
    {
        if (audio == null)
            gameObject.AddComponent<AudioSource>();
    }


    private void OnClick()
    {
        if (_clips.Length == 0)
        {
            audio.PlayOneShot(audio.clip);
            return;
        }
        audio.clip = _clips[Random.Range(0, _clips.Length)];
        audio.PlayOneShot(audio.clip);
    }
}
