using UnityEngine;

public class Water : MonoBehaviour {
    [SerializeField] private Vector3 newGravity = new Vector3(0, -5, 0);
    [SerializeField] private float stopForce = 0.1f; //tmp

    private Vector3 oldGravity = Vector3.zero;

    private PlayerMovment _plMovment = null;

    void Start() {
        _plMovment = ScheneManager.Instance.PlayerInstance.GetModule<PlayerMovment>();        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            oldGravity = _plMovment.Gravity;
            _plMovment.Gravity = newGravity;
            _plMovment.rb.linearVelocity *= stopForce;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            _plMovment.Gravity = oldGravity;
        }
    }
}
