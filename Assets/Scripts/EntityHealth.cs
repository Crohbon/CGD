using UnityEngine;

public class EntityHealth : MonoBehaviour {
    [SerializeField] private float _hitCooldown = 0.5f;
    [SerializeField] private float _baseHealth = 10f;

    public float BaseHealth => _baseHealth;

    public float CurrentHealth { get; private set; }
    public int PlayerIndex;

    private float _lastHitTime = -1f;
    

    private void Awake() {
        CurrentHealth = BaseHealth;
    }

    private void OnEnable() {
        GameEvents.Instance.playerHit.AddListener(HandleEntityHit);
    }

    private void OnDisable() {
        GameEvents.Instance?.playerHit.RemoveListener(HandleEntityHit);
    }

    private void HandleEntityHit(EntityHealth entityHealth, float damage) {
        if(_lastHitTime + _hitCooldown > Time.time) return;
        if (entityHealth.PlayerIndex != PlayerIndex) return;

        CurrentHealth -= damage;
        _lastHitTime = Time.time;
        
        if ((CurrentHealth > 0f)) return;
        
        GameEvents.Instance.OnPlayerDeath(PlayerIndex);
        Destroy(gameObject);
    }
}