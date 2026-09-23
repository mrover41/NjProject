using System.Collections;
using UnityEngine;

public class PlayerToPlaneGravity : MonoBehaviour {
    [SerializeField] private float _garavityScale = 9.8f;
    [SerializeField] private float _rspeed = 90f;

    private PlayerMovment _playerMov = null;
    private Coroutine _rotationCoroutine;

    void Start() {
        _playerMov = ScheneManager.Instance.PlayerInstance.GetModule<PlayerMovment>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            _playerMov.Gravity = -this.gameObject.transform.up * _garavityScale;
            _rotationCoroutine = StartCoroutine(RotateObjectToTarget(other.GetComponent<Collider>().gameObject.transform, this.transform.rotation, _rspeed));
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, -this.gameObject.transform.up * _garavityScale);
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
