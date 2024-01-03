using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class EntityHealth : MonoBehaviour {
    [SerializeField] private bool _isEnemy;
    public float BaseHealth = 10f;
    public float ModifiedMultiplayerHealth = 2f;
    public float _currentHealth { get; private set; }
    [SerializeField] private GameObject _healthBar;
    [SerializeField] private float _hitCooldown = 0.5f;
    private float _lastHitTime = -1f;

    private void Awake() {
    }

    private void OnEnable() {
        GameEvents.Instance.playerHit.AddListener(HandleEntityHit);
    }

    private void OnDisable() {
        GameEvents.Instance?.playerHit.RemoveListener(HandleEntityHit);
    }

    private void HandleEntityHit(EntityHealth entityHealth, float damage) {
        if(_lastHitTime + _hitCooldown > Time.time) return;
        if (!entityHealth.Equals(this)) return;

        _currentHealth -= damage;
        _lastHitTime = Time.time;
        
        if ((_currentHealth > 0f)) return;
        
        GameEvents.Instance.OnPlayerDeath(gameObject.GetComponent<PlayerInput>().playerIndex);
        Destroy(gameObject);
    }
}