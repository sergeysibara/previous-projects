using UnityEngine;
using System.Collections;

public class SmoothCamera : MonoBehaviour
{
	[SerializeField]
	private float _speed = 500f;

	[SerializeField]
	Vector2 _distanceClamp = new Vector2(5, 20);

	[SerializeField]
	public float _movementSmoothTime = 0.5f;

	private float _movementAxel;
	private float _currentMovementSpeed;

	private void Update()
	{
		SmoothChangeDistance(Time.deltaTime);
	}

	private void SmoothChangeDistance(float deltatime)
	{
		var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, LayerMask.GetMask(Consts.Layers.Table)))
			return;

		_currentMovementSpeed = Mathf.SmoothDamp(_currentMovementSpeed, Input.GetAxis("Mouse ScrollWheel") * deltatime, ref _movementAxel, _movementSmoothTime);

		var newDist = hitInfo.distance + _currentMovementSpeed * _speed;

		var clampedDist = Mathf.Clamp(newDist, _distanceClamp.x, _distanceClamp.y);
		var distDiff = hitInfo.distance - clampedDist;
		transform.position += distDiff * ray.direction;

		if (!Mathf.Approximately(clampedDist, newDist))
		{
			_movementAxel = 0;
			_currentMovementSpeed = 0;
		}
	}

}