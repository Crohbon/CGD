using UnityEngine;

public class Bullet : MonoBehaviour {
    public float Damage;
    public float BulletSpeed;

    private Transform _bulletTransform;
    
    private void Awake() {
        _bulletTransform = transform;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")){
            GameEvents.Instance.OnPlayerHit(other.GetComponent<EntityHealth>(), Damage);
        }
        Destroy(gameObject);
    }

    private void Update() {
        _bulletTransform.position += BulletSpeed * Time.deltaTime * _bulletTransform.up;
    }
}
