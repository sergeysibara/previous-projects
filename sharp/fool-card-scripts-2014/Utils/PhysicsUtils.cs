using UnityEngine;

namespace Utils
{
	public static class PhysicsUtils
	{
		public const float RaycastHeigthFromUpToDown = 1000;

		public static RaycastHit RaycastFromUpToDown(Vector3 position)
		{
			RaycastHit hit;
			position.y = RaycastHeigthFromUpToDown;
			Ray ray = new Ray(position, -Vector3.up);
			Physics.Raycast(ray, out hit, Mathf.Infinity);
			return hit;
		}
	}
}