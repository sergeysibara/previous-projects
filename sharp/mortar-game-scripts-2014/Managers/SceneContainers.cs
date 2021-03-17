using UnityEngine;

public class SceneContainers: RequiredMonoSingleton<SceneContainers>
{
    [SerializeField]
    private Transform _units;

    [SerializeField]
    private Transform _targets;

    public static Transform Units
    {
        get { return Instance._units; }
    }


    public static Transform Targets
    {
        get { return Instance._targets; }
    }
}
