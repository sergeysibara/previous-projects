using UnityEngine;

namespace Utils
{
	public static class TransformUtils
	{
		public static string GetFullPath(Transform transform)
		{
			string path = "/" + transform.name;
			while (transform.parent != null)
			{
				transform = transform.parent;
				path = "/" + transform.name + path;
			}
			return path;
		}
	}
}