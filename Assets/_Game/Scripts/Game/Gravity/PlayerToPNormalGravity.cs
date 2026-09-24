using System.Collections;
using UnityEngine;

public class PlayerToPNormalGravity : MonoBehaviour {
    [SerializeField] private float _garavityScale = 9.8f;
    [SerializeField] private float _rspeed = 90f;

    private PlayerMovment _playerMov = null;
    private static Coroutine _rotationCoroutine;

    void Start() {
        _playerMov = ScheneManager.Instance.PlayerInstance.GetModule<PlayerMovment>();
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player")) {
            if (other.contactCount < 0) return;
            Vector3 surfaceNormal = other.contacts[0].normal;
            _playerMov.Gravity = surfaceNormal * _garavityScale;
            Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, -surfaceNormal);
            if (_rotationCoroutine != null) StopCoroutine(_rotationCoroutine);
            _rotationCoroutine = StartCoroutine(RotateObjectToTarget(other.gameObject.GetComponent<Collider>().gameObject.transform, surfaceRotation, _rspeed));
        }
    }

    private IEnumerator RotateObjectToTarget(Transform targetTransform, Quaternion targetRotation, float speed) {
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
    }
}
