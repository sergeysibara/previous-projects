using UnityEngine;

struct VectorInt2
{
    public int x;
    public int y;

    public VectorInt2(int valueX, int valueY)
    {
        x = valueX;
        y = valueY;
    }

    public static implicit operator VectorInt2(Vector3 value)
    {
        int x = Mathf.RoundToInt(value.x);
        int y = Mathf.RoundToInt(value.z);
        return new VectorInt2(x, y);
    }

}
