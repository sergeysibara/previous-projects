using UnityEngine;
using System.Collections;

public interface IMySocial
{
    int MaxVisibleScores { get; }

    void SubmitScore(long score);

    /// <param name="aroundMyRankResults">прислать только результаты, вокруг текущего игрока</param>
    /// <param name="count">число видимых результатов</param>
    void LoadScoresForLeaderboard(bool aroundMyRankResults, int count = 0);

    void ShowLeaderboard();

    void Authenticate();

    GPGPlayerInfo GetLocalPlayerInfo();
}
