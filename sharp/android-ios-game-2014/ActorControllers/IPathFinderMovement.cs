using System;
using UnityEngine;


/// <summary>
/// Описывает методы и свойства для контроллеров перемещения с наличием поиска пути.
/// </summary>
interface IPathFinderMovement
{
    void StartMovement(Transform target, Action onMovementStart, Action onPathTraversed);
    void CancelMovement();
    Action OnWaypointChange { set; }
}
