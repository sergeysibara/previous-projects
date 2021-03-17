using UnityEngine;
using System.Collections;
using Pathfinding;

public class UnitBehaviour_Retreat : BaseUnitBehaviour
{
    void Start()
    {
        
    }

    void OnEnable()
    {
        var points = SpawnManager.Instance.GetAllLoopedSpawnPoints();
        MyRichAI richAi = GetComponent<MyRichAI>();
        richAi.target = Getters.TargetFinders.GetNearest(transform.position, points);
        richAi.StartMove();
    }

}
