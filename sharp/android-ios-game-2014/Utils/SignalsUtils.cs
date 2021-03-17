using Shapes;
using UnityEngine;
using System.Collections;
using System.Linq;
public static class SignalsUtils 
{
    /// <summary>
    /// Получение сигналов в клетке с переданными координатами
    /// </summary>    
    public static Signal[] GetSignalsInCell(int x, int y)
    {
        return Physics.OverlapSphere(new Vector3(x, Signal.PosY, y), 0.5f, LayerMaskExt.Create(Consts.Layers.Signals))
                      .Select(c => c.GetComponent<Signal>()).ToArray();
    }

    /// <summary>
    /// Удаление сигналов в клетке с переданными координатами
    /// </summary>    
    public static void DestroySignalsInCell(int x, int y)
    {
        var signals = GetSignalsInCell(x, y);
        for (int i = 0; i < signals.Length; i++)
        {
            signals[i].DestroySelf();
        }
    }

}
