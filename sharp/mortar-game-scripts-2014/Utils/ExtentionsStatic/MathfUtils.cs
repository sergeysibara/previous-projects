using System;
using System.Collections;

namespace UnityEngine
{
    public static class MathfUtils
    {
        public static float Frac(float value)
        {
            var ret = value - (float) Math.Truncate(value);
            if (ret > 0.9999f)
                ret = 0;
            return ret;
        }

        /// <summary>
        /// ��������� ������� ����� � �����. �.�. ��� (0.2) ������ 0.2*10=2
        /// </summary>
        public static int FracDecimalToCeil(float value)
        {
            //pow(10,n)
            return (int) Mathf.RoundToInt(Frac(value)*10f);
        }

        /// <summary>
        /// ������� �� SmoothDamp - �������� �������� ���������, � �� ����� �� ������� ������ ����������
        /// </summary>
        public static float SmoothChangeValue(float currentValue, float targetValue, float changingSpeed, float deltaTime, float min, float max)
        {
            float retValue = currentValue;
            if (currentValue > targetValue)
            {
                retValue -= changingSpeed*deltaTime;
                if (retValue < targetValue) //���� ����� ���������, �������� ����� ������ ��������
                    retValue = targetValue;
            }
            else if (currentValue < targetValue)
            {
                retValue += changingSpeed*deltaTime;
                if (retValue > targetValue) //���� ����� ���������, �������� ����� ������ ��������
                    retValue = targetValue;
            }

            retValue = Mathf.Clamp(retValue, min, max);
            return retValue;
        }

        public static IEnumerator LerpWithDuration(float from, float to, float duration, Action<float> setterFunc, Action OnComplete = null)
        {
            float progress = 0;
            while (progress < 1)
            {
                progress += Time.deltaTime / duration;
                setterFunc(Mathf.Lerp(from, to, progress));
                yield return null;
            }
            if (OnComplete != null)
                OnComplete();
        }
    }
}
