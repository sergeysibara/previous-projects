using Pathfinding;
using UnityEngine;
using System.Collections;

public class BaseUnitBehaviour : MonoBehaviour
{
    protected UnitMediator _mediator;

    protected virtual void Awake()
    {
        _mediator = GetComponent<UnitMediator>();

    }

    protected virtual void OnDisable()
    {
        StopAllCoroutines();
    }
}
