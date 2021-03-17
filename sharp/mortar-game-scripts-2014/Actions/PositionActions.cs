using System;
using UnityEngine;
using System.Collections;

namespace Actions
{
    public static class Position
    {
        public static void MoveToDirection(Transform self, Vector3 normalizedDirection, float speed)
        {
            self.position += (normalizedDirection * Time.deltaTime * speed);
        }

        public static IEnumerator ConstantMoveCoroutine(Vector3 normalizedDirection, float speed, Action<Vector3> setterFunc)
        {
            while (true)
            {
                setterFunc(normalizedDirection * Time.deltaTime * speed);
                yield return null;
            }
        }

        /// <summary>
        /// Вращение с замедлением скорости
        /// </summary>
        public static void ShootdownRotate(Transform self, Vector3 direction, float rotationSpeed)
        {
            if (direction != Vector3.zero)
                self.rotation = Quaternion.Lerp(self.rotation, Quaternion.LookRotation(direction), rotationSpeed*Time.deltaTime);
        }

        /// <summary>
        /// Вращение к цели с постоянной скоростью
        /// </summary>
        /// <param name="rotationSpeed">Скорость в градусах в секунду </param>
        public static void ConstantRotate(Transform self, Vector3 direction, float rotationSpeed)
        {
            if (direction != Vector3.zero)
            {
                self.rotation = Quaternion.RotateTowards(self.rotation, Quaternion.LookRotation(direction), rotationSpeed*Time.deltaTime);
            }
        }

        /// <summary>
        /// Вращение к цели с постоянной скоростью
        /// </summary>
        /// <param name="rotationSpeed">Скорость в градусах в секунду </param>
        /// <param name="endRotationAngle">угол между текущим и целевым объектами, при котором вращение прекратиться</param>
        /// <param name="OnComplete">Действие, которое нужно выполнить при завершении вращения</param>
        public static IEnumerator RotateToTargetCoroutine(Transform self, Transform target, float rotationSpeed, float endRotationAngle, Action OnComplete)
        {
            while (Getters.Base.GetAngleByXZ(self, target) > endRotationAngle)
            {
                Vector3 direction = Getters.Base.GetDirectionByXZ(self.position, target.position);
                self.rotation = Quaternion.RotateTowards(self.rotation, Quaternion.LookRotation(direction), rotationSpeed*Time.deltaTime);
                yield return null;
            }
            OnComplete();
        }
    }
}