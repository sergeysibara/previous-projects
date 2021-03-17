using UnityEngine;
using System.Collections;

public class HandController : MonoBehaviour
{
    private const float _clickInterval = 3f;

	void Start ()
	{
        EventMessenger.Subscribe(GameEvent.OnTutorialCompleteShowText, this, () => Invoke("ShowSprite", _clickInterval));
	}
	
	void Update () {}

    private void ShowSprite()
    {
        var sprite = GetComponent<UISprite>();
        sprite.enabled = true;
    }
}
