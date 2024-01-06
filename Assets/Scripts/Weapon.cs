using System;
using TarodevController;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour {

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float weaponDmg;
    [SerializeField] private float shotCd;
    [SerializeField] private float ammo;
    [SerializeField] private string weaponName;
    
    private float _lastShotTime;

    private Transform _weaponTransform;
    private int _attachedPlayerId = -1;

    private void Awake() {
        _weaponTransform = transform;
    }

    private void OnTriggerEnter2D(Collider2D col) {
        Debug.Log("triggered");
        if (!col.CompareTag("Player") || _attachedPlayerId != -1) return;
        PlayerController player = col.GetComponent<PlayerController>();
        if (player.IsHoldingWeapon) return;
        
        _weaponTransform.SetParent(col.transform);
        _weaponTransform.localPosition = Vector3.zero;
        _weaponTransform.up = player.transform.right * player.transform.localScale.x;
        
        _attachedPlayerId = player.gameObject.GetComponent<PlayerInput>().playerIndex;
        GameEvents.Instance.OnWeaponPickUp(_attachedPlayerId, this);
    }

    public void ShootWeapon() {
        if (Time.time - _lastShotTime > shotCd){
            GameObject spawnedBullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, transform.rotation);
            bulletPrefab.GetComponent<Bullet>().Damage = weaponDmg;
            ammo--;
            if (ammo <= 0){
                GameEvents.Instance.OnWeaponIsEmpty(_attachedPlayerId);
                Destroy(gameObject);
            }

            _lastShotTime = Time.time;
        }
        else{
            Debug.Log(weaponName + " is on cooldown.");
        }
    }
}
