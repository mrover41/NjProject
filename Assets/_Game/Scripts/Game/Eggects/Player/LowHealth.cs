using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LowHealth : MonoBehaviour {
    [SerializeField] private Volume _volume;
    [SerializeField] private float minValue = 1f;
    [SerializeField] private float maxValue = 5f;
    [SerializeField] private float speed = 2f;

    private Vignette _vg;

    void Start() {
        if (!_volume.profile.TryGet<Vignette>(out _vg)) {
            Debug.LogError("Effcet not found in the volume profile.");
        } else {
            _vg.intensity.overrideState = true;
        }
    }

    void Update() {
        _vg.intensity.value = Mathf.PingPong(Time.time * speed, maxValue - minValue) + minValue;
    }
}
