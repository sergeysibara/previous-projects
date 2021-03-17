using UnityEngine;
using System.Collections;

public static class SaveManager  
{
    private const string _starsPrefixKey = "starsCount";
    private const string _scorePrefixKey = "score";

    public static int LoadStarsCount(int levelNumber)
    {
        //Clear();
        return PlayerPrefs.GetInt(_starsPrefixKey + levelNumber);
    }

    public static int LoadScoreCount(int levelNumber)
    {
        return PlayerPrefs.GetInt(_scorePrefixKey + levelNumber);
    }

    public static void SaveStarsCount(int starsCount, int levelNumber)
    {
        var prevResult = LoadStarsCount(levelNumber);
        var newResult = Mathf.Max(prevResult, starsCount);

        PlayerPrefs.SetInt(_starsPrefixKey + levelNumber, newResult);
    }

    public static void SaveScoreCount(int scoreCount, int levelNumber)
    {
        var prevResult = LoadScoreCount(levelNumber);
        var newResult = Mathf.Max(prevResult, scoreCount);

        PlayerPrefs.SetInt(_scorePrefixKey + levelNumber, newResult);
    }

    public static void Save()
    {
        PlayerPrefs.Save();
       
    }

    public static void Clear()
    {
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.DeleteKey(_starsPrefixKey + i);
            PlayerPrefs.DeleteKey(_scorePrefixKey + i);
        }

    }
}
