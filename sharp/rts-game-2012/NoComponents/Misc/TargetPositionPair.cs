using UnityEngine;

public struct TargetPositionPair
{
    public Vector3 NearestBoundaryNodePosition; //позиция граничной ноды, которая определена как цель
    public Vector3 FollowPosition; //ближайшая точка к NearestBoundaryNodePosition, с которой возможна атака и к которой будет следовать юнит.
}
