/// <summary>
/// Отвечает за подсчет очков и определение - на сколько звезд пройден уровень
/// </summary>
public static class ScoreCounter
{
    const int _minOneStarScore = 164;//8*8+100
    const int _minTwoStarsScore = 424;//18*18+100
    const int _minThreeStarsScore = 725;//25sec*25sec+100

    const float _baseStartTime = 30f;//величина времени, относительно которой рассчитывается во сколько раз отличается стартовое время уровня

    /// <summary>
    /// Функция рассчитана на то, что 1000 - максимальное значение очков
    /// </summary>
    public static int TimeToStore(float startTime, float remainTime)
    {
        float k = startTime / _baseStartTime;

        //if (remainTime/k >= 25f)
        //    return 1000;

        float a = 1f / k / k;
        float v = a * remainTime * remainTime; //f(x)=a*x^2
        return (int)(v + 99);
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