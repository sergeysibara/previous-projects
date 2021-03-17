using UnityEngine;

namespace Consts
{
    public static class LayerMasks
    {   
        /// <summary>
        /// Коллизии для камеры (тока terrain). Камера может проходить сквозь камни,деревья, невысокие сооружения, т.к. она довольно высоко расположена.
        /// </summary>
        public static readonly LayerMask GroundForCamera = LayerMaskExt.Create(Layers.Ground);

        /// <summary>
        /// Коллизии для ядра пушки (земля, + некоторые объекты, например камни)
        /// </summary>
        public static readonly LayerMask BallCollisions = LayerMaskExt.Create(Layers.Ground);

        /// <summary>
        /// Коллизии для юнитов, спавн поинтов (земля, + некоторые объекты, например камни)
        /// </summary>
        public static readonly LayerMask GroundForUnits = LayerMaskExt.Create(Layers.Ground);

        public static readonly LayerMask Units = LayerMaskExt.Create(Layers.Unit);
        public static readonly LayerMask UI = LayerMaskExt.Create(Layers.UI);
    }
}
