using UnityEngine;

public class Damage : MonoBehaviour {
    private PlayerHealth _phealth;

    [SerializeField] private float damage = 50;

    private void Start() {
        _phealth = ScheneManager.Instance.PlayerInstance.GetComponent<PlayerHealth>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            _phealth.Hurt(damage);
        }
    }
}
