using UnityEngine;

public class MySocialMain : MonoSingleton<MySocialMain>, IMySocial
{
    //[SerializeField]
    private IMySocial _currentPlugin;

    private void Start()
    {
#if UNITY_ANDROID
        _currentPlugin = MySocialPrime31.Instance;
#endif

#if UNITY_IPHONE
        _currentPlugin = MySocialNative.Instance;
#endif
        Authenticate();
    }

    public int MaxVisibleScores
    {
        get
        {
            return _currentPlugin.MaxVisibleScores;
        }
    }

    public void SubmitScore(long score)
    {
        _currentPlugin.SubmitScore(score);
    }

    public void LoadScoresForLeaderboard(bool aroundMyRankResults, int count = 0)
    {
       _currentPlugin.LoadScoresForLeaderboard(aroundMyRankResults, count);
    }

    public void ShowLeaderboard()
    {
        _currentPlugin.ShowLeaderboard();
    }

    public void Authenticate()
    {
        _currentPlugin.Authenticate();
    }


    public GPGPlayerInfo GetLocalPlayerInfo()
    {
        return _currentPlugin.GetLocalPlayerInfo();
    }
}
