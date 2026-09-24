using System.Collections;
using UnityEngine;

public class PlayerToPNormalGravity : MonoBehaviour {
    [SerializeField] private float _garavityScale = 9.8f;
    [SerializeField] private float _rspeed = 90f;
    [SerializeField] private bool _update = false;

    private Collision _playerCollider = null;
    private Transform _playerTransform = null;
    private bool _doneToUpdateRotation = false;

    private PlayerMovment _playerMov = null;
    private static Coroutine _rotationCoroutine;

    void Start() {
        _playerMov = ScheneManager.Instance.PlayerInstance.GetModule<PlayerMovment>();
    }

    void Update() {
        if (!_update) return;
        if (_playerCollider == null || _playerTransform == null) return;

        Vector3 surfaceNormal = ClalulateSurfaceNormal(_playerCollider);
        Quaternion surfaceRotation = ClalulateTargetRotation(surfaceNormal);

        Vector3 targetUp = surfaceRotation * Vector3.up;
        if (!_doneToUpdateRotation && Vector3.Angle(_playerTransform.up, targetUp) < 0.1f) _doneToUpdateRotation = true;

        _playerMov.Gravity = surfaceNormal * _garavityScale;

        if (!_doneToUpdateRotation) RotateUpdate(_playerTransform, surfaceRotation, _rspeed);
        else RotateUpdate(_playerTransform, surfaceRotation, 0, true);
    }

    private void OnCollisionStay(Collision other) {
        if (!_update) return;
        if (other.gameObject.CompareTag("Player")) _playerCollider = other;
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player")) {
            _playerCollider = other;
            _playerTransform = other.gameObject.GetComponent<Collider>().gameObject.transform;
            if (_update) return;
            
            Vector3 surfaceNormal = ClalulateSurfaceNormal(other);
            Quaternion surfaceRotation = ClalulateTargetRotation(surfaceNormal);

            _playerMov.Gravity = surfaceNormal * _garavityScale;
            if (_rotationCoroutine != null) StopCoroutine(_rotationCoroutine);
            _rotationCoroutine = StartCoroutine(RotateObjectToTarget(_playerTransform, surfaceRotation, _rspeed));
        }
    }

    private void OnCollisionExit(Collision other) {
        if (other.gameObject.CompareTag("Player")) {
            _doneToUpdateRotation = false;
            _playerCollider = null;
        } 
    }

    private Vector3 ClalulateSurfaceNormal(Collision other) {
        if (other.contactCount < 0) return Vector3.zero;
        return other.contacts[0].normal;
    }

    private Quaternion ClalulateTargetRotation(Vector3 normal) {
        return Quaternion.FromToRotation(Vector3.up, -normal);
    }

    /*private IEnumerator RotateObjectToTarget(Transform targetTransform, Quaternion targetRotation, float speed) {
        if (targetTransform == null) yield break;

        Vector3 targetUp = targetRotation * Vector3.up;

        while (Vector3.Angle(targetTransform.up, targetUp) > 0.1f) {
            if (targetTransform == null) yield break;

            Vector3 currentUp = Vector3.RotateTowards(
                targetTransform.up, 
                targetUp, 
                speed * Mathf.Deg2Rad * Time.deltaTime, 
                0f
            );

            Quaternion alignRotation = Quaternion.FromToRotation(targetTransform.up, currentUp);
            targetTransform.rotation = alignRotation * targetTransform.rotation;

            yield return null;
        }

        targetTransform.rotation = Quaternion.FromToRotation(targetTransform.up, targetUp) * targetTransform.rotation;
    }*/

    private IEnumerator RotateObjectToTarget(Transform targetTransform, Quaternion targetRotation, float speed) {
        if (targetTransform == null) yield break;

        Vector3 targetUp = targetRotation * Vector3.up;

        while (Vector3.Angle(targetTransform.up, targetUp) > 0.1f) {
            if (targetTransform == null) yield break;

            RotateUpdate(targetTransform, targetRotation, speed);

            yield return null;
        }

        targetTransform.rotation = Quaternion.FromToRotation(targetTransform.up, targetUp) * targetTransform.rotation;
    }

    private void RotateUpdate(Transform targetTransform, Quaternion targetRotation, float speed, bool ignoreSpeed = false) {
        Vector3 targetUp = targetRotation * Vector3.up;

        if (ignoreSpeed) {
            Vector3 projectedForward = Vector3.ProjectOnPlane(targetTransform.forward, targetUp).normalized;
            if (projectedForward.sqrMagnitude < 0.001f) projectedForward = Vector3.ProjectOnPlane(targetTransform.right, targetUp).normalized;
            targetTransform.rotation = Quaternion.LookRotation(projectedForward, targetUp);
            return;
        }

        Vector3 currentUp = Vector3.RotateTowards(
            targetTransform.up, 
            targetUp, 
            speed * Mathf.Deg2Rad * Time.deltaTime, 
            0f
        );

        Quaternion alignRotation = Quaternion.FromToRotation(targetTransform.up, currentUp);
        targetTransform.rotation = alignRotation * targetTransform.rotation;

        //targetTransform.rotation = Quaternion.FromToRotation(targetTransform.up, targetUp) * targetTransform.rotation;
    }
}
