using System;
using UnityEngine;
using System.Collections;

public class LabelTimer : MonoBehaviour
{
    [SerializeField]
    private float _startTime = 30f;

    private UILabel[] _label =new UILabel[3];

    public float RemainTime { get; private set; }
    public float StartTime { get { return _startTime; } }

    void Start ()
    {
        _label[0] = transform.FindChild("Label1").GetSafeComponent<UILabel>();
        _label[1] = transform.FindChild("Label2").GetSafeComponent<UILabel>();
        _label[2] = transform.FindChild("Label3").GetSafeComponent<UILabel>();

	    RemainTime = _startTime;

        EventMessenger.Subscribe(GameEvent.StartGameProcess, this, () => StartCoroutine(UpdateTimeCoroutine(0.1f)));
        EventMessenger.Subscribe(GameEvent.EngGameProcess, this, StopAllCoroutines);
	}

    private IEnumerator UpdateTimeCoroutine(float frequency) //0.1f
    {
        RemainTime = _startTime;
        SetText(RemainTime < 10 ? "0" + RemainTime.ToString() : RemainTime.ToString(), ":", "0");

        while (RemainTime > 0)
        {
            yield return new WaitForSeconds(frequency);
            RemainTime -= frequency;
            int remainSec = (int) Mathf.Floor(RemainTime + 0.01f); //+0.01f для исправления погрешности float
            int remainMs = MathfUtils.FracDecimalToCeil(RemainTime);
            SetText(remainSec < 10 ? "0" + remainSec.ToString() : remainSec.ToString(), ":", remainMs.ToString());
        }
        if (RemainTime <= 0)
        {
            SetText("00", ":", "0");
        }
    }

    private void SetText(string label1, string label2, string label3)
    {
        _label[0].text = label1;
        _label[1].text = label2;
        _label[2].text = label3;
    }
}
