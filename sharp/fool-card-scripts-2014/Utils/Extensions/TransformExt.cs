using UnityEngine;

public static class TransformExt
{
    public static void SetX(this Transform transform, float x)
    {
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    public static void SetY(this Transform transform, float y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    public static void SetZ(this Transform transform, float z)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }

    public static void SetLocalX(this Transform transform, float x)
    {
        transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
    }

    public static void SetLocalY(this Transform transform, float y)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
    }

    public static void SetLocalZ(this Transform transform, float z)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
    }

    public static void AddX(this Transform transform, float x)
    {
        transform.position += new Vector3(x, 0, 0);
    }

    public static void AddY(this Transform transform, float y)
    {
        transform.position += new Vector3(0, y, 0);
    }

    public static void AddZ(this Transform transform, float z)
    {
        transform.position += new Vector3(0, 0, z);
    }

    public static void SetRotationEulerY(this Transform transform, float y)
    {
        transform.rotation =
            Quaternion.Euler(transform.rotation.eulerAngles.x, y, transform.rotation.eulerAngles.z);
    }
    public static void SetRotationEulerX(this Transform transform, float x)
    {
        transform.rotation =
            Quaternion.Euler(x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
    public static void SetRotationEulerZ(this Transform transform, float z)
    {
        transform.rotation =
            Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, z);
    }

	public static void SetScaleZ(this Transform transform, float z)
	{
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, z);
	}

}
