using UnityEngine;
using System.Collections;

namespace Conditions
{  
    public static class Application
    {
        public static bool IsBattleScene
        {
            get { return UnityEngine.Application.loadedLevelName.Contains(Getters.Application.BattleScenePrefixName); }
        }
    }

}