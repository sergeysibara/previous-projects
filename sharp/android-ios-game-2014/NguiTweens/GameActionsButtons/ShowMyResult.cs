using UnityEngine;
using System.Collections;

public class ShowMyResult : MonoBehaviour
{
    private void Start()
    {
    }

    private void OnClick()
    { 
        MySocialMain.Instance.LoadScoresForLeaderboard(true);
        //test only
        //var sr = FindObjectOfType<ScoreResults>();
        //sr.FillTable(sr.CreateTestItems(5));
    }
}
