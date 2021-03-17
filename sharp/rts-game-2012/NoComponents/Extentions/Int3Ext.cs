using Pathfinding;

namespace UnityEngine
{
    public static class Int3Ext
    {
        public static Vector3 ToVector3(this Int3 int3)
        {
            return new Vector3(int3.x * 0.001f, int3.y * 0.001f, int3.z * 0.001f);
        }
    }
}
