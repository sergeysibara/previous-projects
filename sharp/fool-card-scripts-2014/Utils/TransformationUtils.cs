using UnityEngine;

namespace Utils
{
	public static class TransformationUtils
	{
		public static void UpdateHeightDependingFromCount(Transform transform, int count, float unitHeight)
		{
			transform.SetScaleZ(count * unitHeight);
		}

		public static void LocateAboveSurface(Transform transform, float yOffset)
		{
			var collider = transform.GetComponent<Collider>();
			float pivotPos = transform.position.y - collider.bounds.min.y; //позиция пивота относительно нижней границы коллайдера
			collider.enabled = false;
			transform.position = PhysicsUtils.RaycastFromUpToDown(transform.position).point + new Vector3(0, pivotPos + 0.01f + yOffset, 0);
			collider.enabled = true;
		}
	}
}
