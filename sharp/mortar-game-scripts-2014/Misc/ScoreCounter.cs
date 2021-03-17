/// <summary>
/// Отвечает за подсчет очков и определение - на сколько звезд пройден уровень
/// </summary>
public static class ScoreCounter
{
    private const int _minOneStarScore = 1500;
    private const int _minTwoStarsScore = 2000;
    private const int _minThreeStarsScore = 2500;

    const float _baseStartTime = 180f;//величина времени, относительно которой рассчитывается во сколько раз отличается стартовое время уровня

    /// <summary>
    /// Вычисляет количество очков, в зависимости от длительности боя. При длительности более 180, очки будут уменьшены пропорционально коэфициенту разницы длительности.
    /// </summary>
    public static int CalculateScore(float startTime, float scoreAtCurrentBattle)
    {
        float k = startTime / _baseStartTime;

        float v = scoreAtCurrentBattle/k;
        return (int)(v);
    }

    public static int GetCountStars(int score)
    {
        if (score >= _minThreeStarsScore)
            return 3;
        if (score >= _minTwoStarsScore)
            return 2;
        if (score >= _minOneStarScore)
            return 1;
        return 0;
    }


}