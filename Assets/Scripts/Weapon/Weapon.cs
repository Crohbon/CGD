using TarodevController;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour {

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float weaponDmg;
    [SerializeField] private float shotCd;
    [SerializeField] private int ammo;
    [SerializeField] private string weaponName;
    
    private float _lastShotTime;

    private Transform _weaponTransform;
    private int _attachedPlayerIndex = -1;

    private void Awake() {
        _weaponTransform = transform;
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (!col.CompareTag("Player") || _attachedPlayerIndex != -1) return;
        PlayerController player = col.GetComponent<PlayerController>();
        if (player.IsHoldingWeapon) return;
        
        _weaponTransform.SetParent(col.transform);
        _weaponTransform.localPosition = new Vector3(0f,-0.3f,0f);
        _weaponTransform.right = player.transform.right * player.transform.localScale.x;
        
        _attachedPlayerIndex = player.gameObject.GetComponent<PlayerController>().PlayerConfiguration.PlayerIndex;
        GameEvents.Instance.OnWeaponPickUp(_attachedPlayerIndex, this);
    }

    public void ShootWeapon(float damageMultiplier, float bulletDropRange) {
        if (Time.time - _lastShotTime > shotCd){
            Bullet spawnedBullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation).GetComponent<Bullet>();
            spawnedBullet.Damage = weaponDmg * damageMultiplier;
            spawnedBullet.BulletDropRange = bulletDropRange;
            
            ammo--;
            GameEvents.Instance.OnWeaponShot(_attachedPlayerIndex);
            
            if (ammo <= 0){
                GameEvents.Instance.OnWeaponIsEmpty(_attachedPlayerIndex);
                Destroy(gameObject);
            }

            _lastShotTime = Time.time;
        }
        else{
            Debug.Log(weaponName + " is on cooldown.");
        }
    }

    public int GetAmmunition() {
        return ammo;
    }
}
