using UnityEngine;

public class DamageArea : MonoBehaviour {
    [SerializeField] private float _damage;
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")){
            GameEvents.Instance.OnPlayerHit(other.GetComponent<EntityHealth>(), _damage);
        }
    }
}
