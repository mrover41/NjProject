using UnityEngine;
using Ext.Math;

public class PlaerCamera : ModuleBase {
    [SerializeField] private float sensitivity = 5f;
    [SerializeField] private float maxAngle = 5f;
    [SerializeField] private float smooth = 25f;
    
    private float zRotation = 0;

    private Transform _cam;
    private Transform _player;
    private PlayerMovment _pMov = null;

    private float _xRotation = 0;

    public override void OnEnable(Player pl) {
        _cam = Camera.main.transform;
        _player = pl.gameObject.transform;
        _pMov = pl.GetModule<PlayerMovment>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void OnUpdate() {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        float targetZ = 0;
        if (_pMov != null) {
            if (/*_pMov.groundCounter <= 0*/ false) {
                targetZ = 0;
            } else {
                Vector3 localVelocity = player.gameObject.transform.InverseTransformDirection(_pMov.rb.linearVelocity);
                targetZ = localVelocity.x.Map(-_pMov.maxSpeed, _pMov.maxSpeed, maxAngle, -maxAngle);
            }
        }

        zRotation = Mathf.Lerp(zRotation, targetZ, smooth * Time.deltaTime);

        _cam.transform.localRotation = Quaternion.Euler(_xRotation, 0f, zRotation);

        _player.Rotate(0f, mouseX, 0f);        
    }
}