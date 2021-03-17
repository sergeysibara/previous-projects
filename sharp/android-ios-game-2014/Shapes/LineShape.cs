using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Shapes
{
    public class LineShape : Shape
    {
        [SerializeField]
        private Transform _upSignalPoint;

        [SerializeField]
        private Transform _downSignalPoint;

        public Vector3 UpSignalPoint { get { return _upSignalPoint.position; } }
        public Vector3 DownSignalPoint { get { return _downSignalPoint.position; } }

        protected override void Awake()
        {
            base.Awake();
            //  |
            //Up = Down = true;
            Sides[(byte)_currentDirection] = true;
            Sides[(byte)_currentDirection.GetOpposite()] = true;
        }

        protected override bool NeedContinueRotating(Direction targetDirection)
        {
            return _currentDirection != targetDirection && _currentDirection != targetDirection.GetOpposite();
        }

        public override Direction GetOutDirection(Direction prevOutDirection)
        {
            return HasConnection(prevOutDirection) ? prevOutDirection : Direction.None; //для линии входное направление предыдущего элемента и выходное направление текущего - одинаковы.
        }

        public override List<Vector3> GetPath(Direction prevOutDirection)
        {
            List<Vector3> path = new List<Vector3>();
            if (prevOutDirection == _currentDirection)
            {               
                path.Add(DownSignalPoint);
                path.Add(UpSignalPoint);
            }
            else if (prevOutDirection.GetOpposite() == _currentDirection)
            {
                path.Add(UpSignalPoint);
                path.Add(DownSignalPoint);
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
    }
}