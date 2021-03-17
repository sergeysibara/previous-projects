using UnityEngine;
using System.Collections;

public class ShowLeaderBoard : MonoBehaviour
{
    private void Start()
    {
    }

    private void OnClick()
    {
        //Debug.LogWarning(PlayGameServices.isSignedIn());
        MySocialMain.Instance.ShowLeaderboard();
    }
}
