using UnityEngine;
using Ext.Math;

public class PlaerCamera : MonoBehaviour {
    [SerializeField] private float sensitivity = 5f;
    [SerializeField] private float maxAngle = 5f;
    [SerializeField] private float maxFov = 90f;
    [SerializeField] private float maxDashFov = 130f;
    [SerializeField] private float minFov = 85f;
    [SerializeField] private float smooth = 25f;
    [SerializeField] private float fovSmooth = 15f;

    private float zRotation = 0;
    private float targetFov = 0;

    private Transform _cam;
    private Camera _ccam;
    private Transform _player;
    private PlayerMovment _pMov = null;

    private float _xRotation = 0;

    private void Start() {
        _ccam = Camera.main;
        _cam = _ccam.transform;
        _player = transform;
        _pMov = GetComponent<PlayerMovment>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        float targetZ = 0;
        if (_pMov != null) {
			Vector3 localVelocity;
			if ((_pMov.Grounded || _pMov.isDashing) && !_pMov.isSlaming) localVelocity = transform.InverseTransformDirection(_pMov.rb.linearVelocity);
			else localVelocity = Vector3.zero;

			targetZ = localVelocity.x.Map(-_pMov.maxSpeed, _pMov.maxSpeed, maxAngle, -maxAngle);

			if (_pMov.isDashing) targetFov = localVelocity.normalized.z.Map(0, 1, minFov, maxDashFov);
            else if (_pMov.Grounded && !_pMov.isSlaming) targetFov = _pMov.input.normalized.z.Map(0, 1, minFov, maxFov);
            else targetFov = _pMov.rb.linearVelocity.magnitude.Map(0, _pMov.maxSpeed * 5, minFov, maxDashFov);
        }

        zRotation = Mathf.Lerp(zRotation, targetZ, smooth * Time.deltaTime);
        _ccam.fieldOfView = Mathf.Lerp(_ccam.fieldOfView, targetFov, fovSmooth * Time.deltaTime);

        _cam.transform.localRotation = Quaternion.Euler(_xRotation, 0, zRotation);
        _player.Rotate(0f, mouseX, 0f);
    }
}
