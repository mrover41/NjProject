using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AutoFocus : MonoBehaviour {
    [SerializeField] private Volume _volume;
    [SerializeField] private LayerMask _hitLayers;
    [SerializeField] private float _maxDistance = 50f;
    
    private DepthOfField _dof;

    void Start() {
        if (_volume.profile.TryGet<DepthOfField>(out _dof)) {
            _dof.focusDistance.overrideState = true;
        } else {
            Debug.LogError("Depth of Field not found in the volume profile.");
        }
    }

    void Update() {
        if (_dof == null) return;

        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance, _hitLayers)) {
            _dof.focusDistance.value = hit.distance;
        } else {
            _dof.focusDistance.value = _maxDistance;
        }
    }
}
