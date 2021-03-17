using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Описывает методы для контроллеров перемещения без поиска пути.
/// </summary>
 interface ISimpleMovement
{
    void StartMovement(Transform target, Action onPathTraversed);
}
