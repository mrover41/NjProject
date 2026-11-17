using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class HealthSlider : MonoBehaviour {
    private Slider _slider;
    private PlayerHealth _phealth;

    [SerializeField] private float changeSpeed = 5;

    void Start() {
        _slider = GetComponent<Slider>();
        _phealth = ScheneManager.Instance.PlayerInstance.GetComponent<PlayerHealth>();
        _slider.maxValue = _phealth.MaxHealth;
    }

    void Update() {
        if (_slider == null || _phealth == null) return;
        _slider.value = Mathf.Lerp(_slider.value, _phealth.CurrentHealth, changeSpeed * Time.deltaTime);        
    }
}
