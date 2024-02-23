using UnityEngine;

public class Bullet : MonoBehaviour {
    public float Damage;
    public float BulletSpeed;
    public float BulletDropRange;
    public string WeaponName;
    public int GunnerIndex;

    private Transform _bulletTransform;
    private float _distanceTraveled = 0f;
    
    private void Awake() {
        _bulletTransform = transform;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")){
            EntityHealth hitEntityHealth = other.GetComponent<EntityHealth>();
            GameEvents.Instance.OnPlayerHit(hitEntityHealth, Damage);
            Logger.LogWeaponImpact(GunnerIndex, WeaponName, Damage, hitEntityHealth.PlayerIndex, (hitEntityHealth.CurrentHealth - Damage) <= 0);
        }
        Destroy(gameObject);
    }

    private void Update() {
        if (BulletDropRange > 0){
            if (_distanceTraveled > BulletDropRange){
                _bulletTransform.position += BulletSpeed * Time.deltaTime * (_bulletTransform.up + (Vector3.down * (1 - (BulletDropRange/_distanceTraveled))));
                _distanceTraveled += (BulletSpeed * Time.deltaTime * _bulletTransform.up).magnitude;
            }
            else{
                _bulletTransform.position += BulletSpeed * Time.deltaTime * _bulletTransform.up;
                _distanceTraveled += (BulletSpeed * Time.deltaTime * _bulletTransform.up).magnitude;
            }
        }
        else{
            _bulletTransform.position += BulletSpeed * Time.deltaTime * _bulletTransform.up;
        }
    }
}
