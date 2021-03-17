using UnityEngine;
using System.Collections;

public class SpriteAlphaChanger : MonoBehaviour
{
    [SerializeField]
    private float _time = 2f;

    private const string _colorPropertyName = "_Color";

    private void Awake()
    {
        var c = renderer.material.GetColor(_colorPropertyName);
        c.a = 0f;
        renderer.material.SetColor(_colorPropertyName, c);
    }

	void Start () {}

    public void StartAlphaChanging()
    {
        StartCoroutine(ChangeAlphaCoroutine());
    }

    private IEnumerator ChangeAlphaCoroutine()
    {
        //System.DateTime t1 = default(System.DateTime), t2 = default(System.DateTime);
        //t1 = System.DateTime.Now;
        var c = renderer.material.GetColor(_colorPropertyName);
        //Debug.LogWarning("t1");
        while (c.a < 1f)
        {
            var speed = Time.deltaTime / _time;  //speed=путь*Time.deltaTime/time
            c.a += speed;
            renderer.material.SetColor(_colorPropertyName, c);
            yield return null;
        }
        //t2 = System.DateTime.Now;
        //Debug.LogWarning("time: "+(t2 - t1).TotalMilliseconds);

        EventMessenger.SendMessage(GameEvent.OnTutorialCompleteShowText, this);
    }

}
