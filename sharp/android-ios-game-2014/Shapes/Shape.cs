using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Shapes
{
    public abstract class Shape : MonoBehaviour
    {
        public readonly bool[] Sides = new bool[4];

        public bool IsInRotateProcess { get; protected set; }

        public Direction CurrentDirection
        {
            get { return _currentDirection; }
        }

        [HideInInspector]
        public int Xindex;

        [HideInInspector]
        public int Yindex;

        /// <remarks>константа, т.к. скорость общая для всех фигур</remarks>
        private const float _rotationSpeed = 400f;

        [SerializeField]//, HideInInspector]
        protected Direction _currentDirection = _defaultStartDirection;

        private const Direction _defaultStartDirection = Direction.Up;


        [SerializeField]
        protected Mesh[] _randomMeshs;


        private float _rotationRemain;
        
        /// <summary>
        /// Положение, которое займет объект, после прекращения вращения на 90 градусов
        /// </summary>
        private float _targetRotationAngle;
        //public Direction CorrectDirection;

        public bool Up
        {
            get { return Sides[0]; }
            protected set { Sides[0] = value; }
        }

        public bool Right
        {
            get { return Sides[1]; }
            protected set { Sides[1] = value; }
        }

        public bool Down
        {
            get { return Sides[2]; }
            protected set { Sides[2] = value; }
        }

        public bool Left
        {
            get { return Sides[3]; }
            protected set { Sides[3] = value; }
        }

        protected virtual void Awake()
        {
            //автоустановка правильного значения _currentDirection при старте игры
            _currentDirection = DirectionUtils.EulerAngleToDirection(transform.rotation.eulerAngles.y);

            var meshFilter = transform.GetComponentInChildren<MeshFilter>();
            meshFilter.mesh = RandomUtils.GetRandomItem(_randomMeshs);
        }

        private void Update()
        {
            if (IsInRotateProcess)
            {
                var rotationDelta = _rotationSpeed * Time.deltaTime;
                _rotationRemain -= rotationDelta;
                if (_rotationRemain >= 0)
                {
                    transform.Rotate(0, -(rotationDelta), 0);
                }
                else
                {
                    transform.SetRotationEulerY(_targetRotationAngle);
                    IsInRotateProcess = false;
                }
            }
        }

        private void OnClick()
        {
            if (audio != null && MainSceneManager.CurrentGameMode == MainSceneManager.GameMode.Normal)
                audio.Play();
            RotateCommand();
        }

        /// <summary>
        /// Команда вращения на 90 градусов, получаемая от другого игрового объекта или игрока. Выполняется не всегда.
        /// </summary>
        public void RotateCommand()
        {
            if (MainSceneManager.CurrentGameMode != MainSceneManager.GameMode.Normal || IsInRotateProcess)
                return;
            RotateToLeft();
        }

        /// <summary>
        /// Команда вращения для установки в заданное направление. Выполняется всегда, даже если shape вращается в данный момент.
        /// </summary>
        public void RotateToDirection(Direction direction)
        {
            StartCoroutine(RotateToDirectionCoroutine(direction));
        }

        public void FastRotate()
        {
            UpdateInnerRotateVariables();
            transform.Rotate(0, -90, 0);
        }

        public void FastRotateToDirection(Direction direction)
        {
            while (NeedContinueRotating(direction))
            {
                FastRotate();
            }
        }

        private void RotateToLeft()
        {
            if (IsInRotateProcess)
                return;
            UpdateInnerRotateVariables();
            _rotationRemain = 90;
            _targetRotationAngle = transform.rotation.eulerAngles.y - 90;
            IsInRotateProcess = true;
            MainSceneManager.OnShapeRotateStart(this);
        }

        private IEnumerator RotateToDirectionCoroutine(Direction direction)
        {
            while (IsInRotateProcess) //если уже вращается (например, если игрок или кот вызвали вращение)
            {
                yield return new WaitForSeconds(0.02f);
            }
            
            while (NeedContinueRotating(direction))
            {
                if (!IsInRotateProcess)
                    RotateToLeft();
                yield return new WaitForSeconds(0.02f);
            }
        }

        /// <summary>
        /// Условие продолжения выполнения корутины RotateToDirectionCoroutine
        /// </summary>
        protected virtual bool NeedContinueRotating(Direction targetDirection)
        {
            return _currentDirection != targetDirection;
        }
        

        private void UpdateInnerRotateVariables()
        {
            _currentDirection = _currentDirection.GetPrev();
            bool tempSide = Sides[0];
            for (int i = 0; i < 3; i++)
                Sides[i] = Sides[i + 1];
            Sides[3] = tempSide;
        }


        #if UNITY_EDITOR
        public void RotateThroughInspector()
        {
            UpdateInnerRotateVariables();
            transform.Rotate(0, -90, 0);
            _currentDirection = DirectionUtils.EulerAngleToDirection(transform.rotation.eulerAngles.y);
        }

        /// <summary>
        /// Используется для корретировки значения в режиме редактора. В коде игры не использовать 
        /// </summary>
        public void UpdateCurrentDirectionInEditorMode()
        {            
            if (!Application.isPlaying)
                _currentDirection = DirectionUtils.EulerAngleToDirection(transform.rotation.eulerAngles.y);
        }
        #endif

        /// <summary>
        /// Проверка наличия соединения
        /// </summary>
        /// <param name="prevOutDirection">Направление выхода предыдущего элемента. Противоположно проверяемой стороне текущего элемента</param>
        public bool HasConnection(Direction prevOutDirection)
        {
            return Sides[(byte)prevOutDirection.GetOpposite()];
        }

        //public bool HasConnection(Shape prevShape)
        //{
        //    return Sides[(byte)_currentDirection.GetOpposite()] && Sides[(byte)prevShape._currentDirection];
        //}

        /// <summary>
        /// Возвращает выходное направление, относительно входного направления.
        /// </summary>
        /// <param name="prevOutDirection">Направление выхода предыдущего элемента. Противоположно проверяемой стороне текущего элемента</param>
        public abstract Direction GetOutDirection(Direction prevOutDirection);

        /// <summary>
        /// Вовзращает точки пути для сигнала, учитывая с какой стороны пришел сигнал
        /// </summary>
        /// <param name="prevOutDirection">Направление выхода предыдущего элемента. Противоположно проверяемой стороне текущего элемента</param>
        public abstract List<Vector3> GetPath(Direction prevOutDirection);

        public abstract List<string> TestPath(Direction prevOutDirection);
    }

}