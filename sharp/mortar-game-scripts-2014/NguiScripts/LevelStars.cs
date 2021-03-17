using UnityEngine;
using System.Collections;

public class LevelStars : MonoBehaviour
{
    [SerializeField]
    private Consts.SceneNames _level;

    private UISprite[] _stars = new UISprite[3];

    void Start()
	{
        string sceneName = _level.GetStringValue<StringValueAttribute>();
        int sceneNum = Getters.Application.GetBattleSceneNumber(sceneName);
        int starsCount = SaveManager.LoadStarsCount(sceneNum);
	    //var sprite = GetComponent<UISprite>();
        var btn = GetComponent<UIButton>();

        _stars[0] = transform.Find("Star1").GetComponent<UISprite>();
        _stars[1] = transform.Find("Star2").GetComponent<UISprite>();
        _stars[2] = transform.Find("Star3").GetComponent<UISprite>();

        switch (starsCount)
        {
            case 1:
                _stars[0].spriteName = "full";
                //btn.normalSprite = "04_level_1_star";
                //sprite.spriteName = "04_level_1_star";
                break;
            case 2:
                _stars[0].spriteName = "full";
                _stars[1].spriteName = "full";
                //btn.normalSprite = "05_level_2_star";
                //sprite.spriteName = "05_level_2_star";
                break;
            case 3:
                _stars[0].spriteName = "full";
                _stars[1].spriteName = "full";
                _stars[2].spriteName = "full";
                //btn.normalSprite = "06_level_3_star";
                //sprite.spriteName = "06_level_3_star";
                break;
        }
        //Debug.LogWarning(starsCount);
	}

}
