using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BtnLoadNextLevel : MonoBehaviour
{
    /// <summary>
    /// Число игровых сцен
    /// </summary>
    private int _levelsNumber;

    private void Start()
    {
    }

    private void OnClick()
    {
        Debug.LogWarning("функция отключена, т.к. нет других сцен");
        //if (Application.loadedLevelName==Consts.SceneNames.Level1.ToString())
        //    Application.LoadLevel(Consts.SceneNames.Level1.ToString());
        //else
        if (Application.loadedLevelName.Contains(Getters.Application.BattleScenePrefixName))
        {
            int level = Getters.Application.GetBattleSceneNumber(Application.loadedLevelName);

            if (level == _levelsNumber)
            {
                //Application.LoadLevel(Consts.SceneNames.Level1.ToString());
                Application.LoadLevel(Getters.Application.BattleScenePrefixName + level);
            }
            else
            {
                level++;
                Application.LoadLevel(Getters.Application.BattleScenePrefixName + level);
            }
        }
    
    }
}
