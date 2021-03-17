using System.Collections.Generic;
using Shapes;
using System;
using UnityEngine;

/// <summary>
/// Элемент цепочки соединененных shape-ов
/// </summary>
[Serializable]
public class ChainItem
{
    public Shape Shape;
    public Direction TargetDirection;

    public List<ChainItem> childChain = new List<ChainItem>();
}
