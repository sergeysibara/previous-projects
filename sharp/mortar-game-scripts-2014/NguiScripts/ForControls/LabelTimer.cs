using System;
using UnityEngine;
using System.Collections;

//todo переделать, т.к. будет без миллисекунд -  min:sec
public class LabelTimer : MonoBehaviour
{
    private UILabel _label;

    public float RemainTime { get; private set; }

    void Start ()
    {
        _label = transform.FindChild("Label1").GetSafeComponent<UILabel>();

        RemainTime = BattleManager.Instance.StartTime;

        EventAggregator.Subscribe(GameEvent.StartGameProcess, this, () => StartCoroutine(UpdateTimeCoroutine(0.1f)));
        EventAggregator.Subscribe<int>(GameEvent.AddTime, this, OnAddTime);
	}

    private IEnumerator UpdateTimeCoroutine(float frequency) //0.1f
    {
        SetText(((int)RemainTime).ToString());

        while (RemainTime > 0)
        {
            yield return new WaitForSeconds(frequency);
            RemainTime -= frequency;
            int remainSec = (int) Mathf.Floor(RemainTime + 0.01f); //+0.01f для исправления погрешности float
            SetText(remainSec.ToString());
        }
        //if (RemainTime <= 0)
        {
            SetText("0");
            EventAggregator.Publish(GameEvent.EngGameProcess, this);
        }
    }

    private void SetText(string label1)
    {
        _label.text = label1;
    }

    private void OnAddTime(int time)
    {
        RemainTime+=time;
    }
}
