using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour {
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float currentHealth = 100;

    public float CurrentHealth {
        get => currentHealth;
        private set => currentHealth = value;
    }

    public float MaxHealth {
        get => maxHealth;
        private set => maxHealth = value;
    }

    private EventHandler? _kill;
    public event EventHandler Kill {
        add => _kill += value;
        remove => _kill -= value;
    }

    public void Heal(float h) {
        currentHealth += Mathf.Clamp(h, 0, maxHealth);
    }

    public void SetHealth(float h) {
        currentHealth = Mathf.Clamp(h, 0, maxHealth);
    }

    public void Hurt(float h) {
        currentHealth -= Mathf.Clamp(h, 0, maxHealth);
        if (currentHealth <= 0) _kill?.Invoke(this, EventArgs.Empty);
    }
}
