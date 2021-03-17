using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Shapes
{
    public class TeeShape : Shape
    {
        [SerializeField]
        private Transform _upSignalPoint;

        [SerializeField]
        private Transform _leftSignalPoint;

        [SerializeField]
        private Transform _rightSignalPoint;

        [SerializeField]
        private Transform _centerSignalPoint;

        public Vector3 UpSignalPoint { get { return _upSignalPoint.position; } }
        public Vector3 LeftSignalPoint { get { return _leftSignalPoint.position; } }
        public Vector3 RightSignalPoint { get { return _rightSignalPoint.position; } }
        public Vector3 CenterSignalPoint { get { return _centerSignalPoint.position; } }


        protected override void Awake()
        {
            base.Awake();
        
            // __|__
            //Left = Up = Right = true;
            Sides[(byte)_currentDirection.GetPrev()] = true;
            Sides[(byte)_currentDirection] = true;
            Sides[(byte)_currentDirection.GetNext()] = true;
        }

        public override Direction GetOutDirection(Direction prevOutDirection)
        {
            if (!HasConnection(prevOutDirection))
                return Direction.None;

            //_currentDirection равна центральной стороне

            if (prevOutDirection.GetOpposite() == _currentDirection) //если входная сторона - центральнная
                return _currentDirection.GetPrev();
            return prevOutDirection; //для двух остальных случаев возвращается сторона, противоположная входной стороне текущего элемента, как и в случае линии.
        }

        /// <summary>
        /// Вторая сторона выхода. Например, для ипользования в клонировании сигнала.
        /// </summary>
        /// <param name="prevOutDirection">Направление выхода предыдущего элемента. Противоположно проверяемой стороне текущего элемента</param>
        public Direction GetSecondOutDirection(Direction prevOutDirection)
        {
            if (!HasConnection(prevOutDirection))
                return Direction.None;

            //_currentDirection равна центральной стороне

            if (prevOutDirection.GetOpposite() == _currentDirection) //если входная сторона - центральнная
                return _currentDirection.GetNext();
            return _currentDirection; //для двух остальных случаев возвращается центральная сторона
        }


        public override List<Vector3> GetPath(Direction prevOutDirection)
        {
            List<Vector3> path = new List<Vector3>();
            if (prevOutDirection.GetOpposite() == _currentDirection) //если входная сторона - центральнная
            {
                path.Add(UpSignalPoint);
                path.Add(CenterSignalPoint);
                path.Add(LeftSignalPoint);
            }
            else if (prevOutDirection.GetOpposite() == _currentDirection.GetPrev())
            {
                path.Add(LeftSignalPoint);
                path.Add(CenterSignalPoint);
                path.Add(RightSignalPoint);
            }
            else if (prevOutDirection.GetOpposite() == _currentDirection.GetNext())
            {
                path.Add(RightSignalPoint);
                path.Add(CenterSignalPoint);
                path.Add(LeftSignalPoint);
            }

            return path;
        }

        /// <summary>
        /// Вовзращает точки пути для клонированного сигнала, учитывая с какой стороны пришел оригинальный сигнал.
        /// </summary>
        /// <param name="inDirection">Направление выхода предыдущего элемента. Противоположно проверяемой стороне текущего элемента</param>
        public List<Vector3> GetSecondPath(Direction inDirection)
        {
            List<Vector3> path = new List<Vector3> {CenterSignalPoint};
            if (inDirection.GetOpposite() == _currentDirection) //если входная сторона - центральнная
            {
                path.Add(RightSignalPoint);
            }
            else if (inDirection.GetOpposite() == _currentDirection.GetPrev() || inDirection.GetOpposite() == _currentDirection.GetNext())
            {
                path.Add(UpSignalPoint);
            }

            return path;
        }

        public override List<string> TestPath(Direction prevOutDirection)
        {
            List<string> path = new List<string>();
            if (prevOutDirection == _currentDirection)
            {
                path.Add("Down");
                path.Add("Up");
            }
            else if (prevOutDirection.GetOpposite() == _currentDirection)
            {
                path.Add("Up");
                path.Add("Down");
            }

            return path;
        }

        private void OnDrawGizmos()//Selected()
        {
           List<Vector3> points=new List<Vector3> {UpSignalPoint, CenterSignalPoint, LeftSignalPoint, RightSignalPoint};

            Gizmos.color = Color.red;
            var p = points[0];
            for (int i = 1; i < points.Count; i++)
            {
                Gizmos.DrawLine(p, points[i]);
                p = points[i];
            }
            foreach (var point in points)
                Gizmos.DrawSphere(point, 0.01f);
        }

    }
}