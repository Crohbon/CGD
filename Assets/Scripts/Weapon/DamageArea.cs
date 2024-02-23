using UnityEngine;

public class DamageArea : MonoBehaviour {
    [SerializeField] private float _damage;
    [SerializeField] private string _areaName;
    
    private void OnTriggerEnter2D(Collider2D other) {
        EntityHealth hitEntityHealth = other.GetComponent<EntityHealth>();
        if (other.CompareTag("Player")){
            GameEvents.Instance.OnPlayerHit(hitEntityHealth, _damage);
            Logger.LogWeaponImpact(hitEntityHealth.PlayerIndex, _areaName, _damage, hitEntityHealth.PlayerIndex, (hitEntityHealth.CurrentHealth - _damage) <= 0);
        }
    }
}
