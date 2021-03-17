using UnityEngine;
using System.Collections;

public class RandomPlayAudio : MonoBehaviour {
    
    [SerializeField]
    private AudioClip[] _clips;

    [SerializeField]
    private bool _playOnStart;

    void Awake()
    {
        if (audio == null)
            gameObject.AddComponent<AudioSource>();
    }

	void Start ()
	{
	    if (_playOnStart)
	        Play();
	}
	
    public void Play()
    {
        audio.clip = _clips[Random.Range(0, _clips.Length)];
        audio.Play();
    }

    public void Stop()
    {
        audio.Stop();
    }
}
