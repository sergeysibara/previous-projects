using UnityEngine;
using System.Collections;

public class ShowTop100 : MonoBehaviour
{
    private void Start()
    {
    }

    private void OnClick()
    {
        MySocialMain.Instance.LoadScoresForLeaderboard(false, 100);
    }
}
