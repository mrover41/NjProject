using UnityEngine;

public class ConfigurePlayer : MonoBehaviour {
    [SerializeField] private bool _enableDash = true;

    private PlayerMovment _plMovment = null;

    void Start() {
        _plMovment = ScheneManager.Instance.PlayerInstance.GetModule<PlayerMovment>();        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            _plMovment.isDashEnabled = _enableDash;
        }
    }
}
