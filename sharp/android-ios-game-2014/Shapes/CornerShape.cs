using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;
namespace Shapes
{
    public class CornerShape : Shape
    {
        private List<Transform> _signalPoints = new List<Transform>();
        private List<Transform> _reversedSignalPoints = new List<Transform>();

        protected override void Awake()
        {
            base.Awake();
            //  |__
            // Up = Right = true;
            Sides[(byte) _currentDirection] = true;
            Sides[(byte) _currentDirection.GetNext()] = true;

            _signalPoints = FindPoints();
            _reversedSignalPoints.AddRange(_signalPoints);
            _reversedSignalPoints.Reverse();
        }

        private List<Transform> FindPoints()
        {
            List<Transform> points = new List<Transform>();
            foreach (Transform tr in transform)
            {
                if (tr.name.ToLower().Contains("point"))
                    points.Add(tr);
            }
  
            //var names=points.OrderBy(p => p.Key).Select(p => p.Key).ToList();
            //names.ForEach(c => Debug.LogWarning(c));

            return  points.OrderBy(t => t.name).ToList();
        }

        private void OnDrawGizmos()//Selected()
        {
            var points = FindPoints();
            Gizmos.color = Color.red;
            var p = points[0].position;
            for (int i = 1; i < points.Count; i++)
            {
                Gizmos.DrawLine(p, points[i].position);
                p = points[i].position;
            }
            foreach (var point in points)
                Gizmos.DrawSphere(point.position, 0.01f);
        }

        public override Direction GetOutDirection(Direction prevOutDirection)
        {
            if (!HasConnection(prevOutDirection))
                return Direction.None;

            // т.к. _currentDirection совпадает с `последней` стороной данной фигуры, то нужно вернуть либо текущую сторону, либо следующую:
            return prevOutDirection.GetOpposite() == _currentDirection ? _currentDirection.GetNext() : _currentDirection;
        }

        public override List<Vector3> GetPath(Direction prevOutDirection)
        {
            List<Vector3> path = new List<Vector3>();
            if (prevOutDirection.GetOpposite() == _currentDirection)
            {
                path.AddRange(_signalPoints.Select(tr=>tr.position));
            }
            else if (prevOutDirection.GetOpposite() == _currentDirection.GetNext()) //(prevOutDirection == _currentDirection.GetPrev())
            {
                //foreach (var p in _reversedSignalPoints)
                //{
                //    Debug.LogWarning(p.name);                    
                //}
                path.AddRange(_reversedSignalPoints.Select(tr => tr.position));
            }

            return path;
        }

        public override List<string> TestPath(Direction prevOutDirection)
        {
            List<string> path = new List<string>();
            if (prevOutDirection.GetOpposite() == _currentDirection)
            {
                path.Add("Up");
                path.Add("Right");                
            }
            else if (prevOutDirection.GetOpposite() == _currentDirection.GetNext())
            {
                path.Add("Right"); 
                path.Add("Up");                 
            }

            return path;
        }
    }
}