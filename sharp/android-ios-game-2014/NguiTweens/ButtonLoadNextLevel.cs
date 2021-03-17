using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ButtonLoadNextLevel : MonoBehaviour
{
    //[SerializeField]
    //private Consts.SceneNames _sceneName;

    private void Start()
    {
    }

    private void OnClick()
    {
        if (Application.loadedLevelName==Consts.SceneNames.Level1.ToString())
            Application.LoadLevel(Consts.SceneNames.Level1.ToString());
        else if (Application.loadedLevelName.Contains("Room"))
        {
            int level = int.Parse(Application.loadedLevelName.Remove(0, 4));

            if (level == 9)
            {
                Application.LoadLevel(Consts.SceneNames.Level1.ToString());
            }
            else
            {
                level++;
                Application.LoadLevel("Room" + level);
            }
        }
    
    }
}
