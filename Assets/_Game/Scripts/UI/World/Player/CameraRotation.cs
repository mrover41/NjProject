using UnityEngine;

public class CameraRotation : MonoBehaviour {
	[SerializeField] private Transform _camera;

	private void LateUpdate() {
		Vector3 currentRotation = transform.localEulerAngles;
		currentRotation.x = _camera.localEulerAngles.x;
		transform.localEulerAngles = currentRotation;
	}
}
