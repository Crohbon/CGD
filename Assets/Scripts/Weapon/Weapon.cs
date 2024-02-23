using UnityEngine;

public class Weapon : MonoBehaviour {

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float weaponDmg;
    [SerializeField] private float shotCd;
    [SerializeField] private float bulletSpeed;
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
        
        Vector3 playerLocalScale = player.transform.localScale;
        _weaponTransform.right = player.transform.right * playerLocalScale.x;
        
        Vector3 weaponLocalsScale = _weaponTransform.localScale;
        _weaponTransform.localScale = new Vector3(weaponLocalsScale.x * Mathf.Abs(playerLocalScale.x/1.5f), 
            weaponLocalsScale.y * (playerLocalScale.y/1.5f), 1); 
        

        _attachedPlayerIndex = player.gameObject.GetComponent<PlayerController>().PlayerConfiguration.PlayerIndex;
        GameEvents.Instance.OnWeaponPickUp(_attachedPlayerIndex, this);
    }

    public void ShootWeapon(float damageMultiplier, float bulletDropRange) {
        if (Time.time - _lastShotTime > shotCd){
            Bullet spawnedBullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation).GetComponent<Bullet>();
            spawnedBullet.Damage = weaponDmg * damageMultiplier;
            spawnedBullet.BulletDropRange = bulletDropRange;
            spawnedBullet.WeaponName = weaponName;
            spawnedBullet.GunnerIndex = _attachedPlayerIndex;
            spawnedBullet.BulletSpeed = bulletSpeed;
            
            ammo--;
            GameEvents.Instance.OnWeaponShot(_attachedPlayerIndex);
            
            Logger.LogWeaponUsage(_attachedPlayerIndex, weaponName, ammo);
            
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
