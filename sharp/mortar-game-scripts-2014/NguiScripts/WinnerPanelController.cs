using UnityEngine;
using System.Collections;

public class WinnerPanelController : MonoBehaviour
{
    [SerializeField]
    private GameObject _lblWinner;

    [SerializeField]
    private GameObject _backGround;

    [SerializeField]
    private float _timeBeforeShow = 3;

	void Start ()
	{
        EventAggregator.Subscribe(GameEvent.EngGameProcess, this, ShowText);
	}

    private void ShowText()
    {
        _lblWinner.SetActive(true);
        Invoke("ShowPanel", _timeBeforeShow);
    }

    private void ShowPanel()
    {
        NGUITools.SetActive(_lblWinner, false);
        NGUITools.SetActive(_backGround, true);
    }



}
