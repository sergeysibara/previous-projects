using UnityEngine;
using System.Collections;

public class RndLevelStars : MonoBehaviour {

	// Use this for initialization
	void Awake () 
    {
        var score=PlayerPrefs.GetInt(Consts.SceneNames.Level1 + "_score");
        var btn = GetComponent<UIButton>();

        switch (ScoreCounter.GetCountStars((int)score))
        {
            case 1:
                btn.normalSprite = "08_level_random_1_star";
                break;
            case 2:
                btn.normalSprite = "08_level_random_2_star";
                break;
            case 3:
                btn.normalSprite = "08_level_random_3_star";
                break;
        }
	}

    private void Start()
    {
    }
}
