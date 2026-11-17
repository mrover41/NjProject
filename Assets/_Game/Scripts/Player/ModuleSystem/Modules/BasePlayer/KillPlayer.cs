using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class KillPlayer : MonoBehaviour {
    [SerializeField] private PlayerHealth _phealth;
    [SerializeField] private int scheneIndx = 0;
    
    private void Start() {
        _phealth = GetComponent<PlayerHealth>();
        if (_phealth == null) return;

        _phealth.Kill += OnKilling;
    }

    private void OnKilling(object? sender, EventArgs ev) {
        SceneManager.LoadScene(scheneIndx, LoadSceneMode.Single);
    }
}
