using UnityEngine;
using System.Collections;

public class ObjectToScreenCenter : MonoBehaviour
{
    [SerializeField]
    private float _positionAboveGround = 10f;

	void Update ()
	{
	    var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity, Consts.LayerMasks.BallCollisions);
        transform.position = new Vector3(hit.point.x, hit.point.y + _positionAboveGround, hit.point.z);
	}
}
