using System.Collections;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour {
    [SerializeField] private float _spawnInterval = 5f;
    [SerializeField] private GameObject _weaponToSpawn;

    private bool _weaponSpawnCoroutineActive;
    private Weapon _currentWeapon;
    
    private void Awake() {
        _currentWeapon = Instantiate(_weaponToSpawn, this.transform).GetComponent<Weapon>();
    }

    private void OnEnable() {
        GameEvents.Instance.weaponPickUp.AddListener(HandleWeaponPickUp);
    }

    private void OnDisable() {
        GameEvents.Instance?.weaponPickUp.RemoveListener(HandleWeaponPickUp);
    }
    
    private void HandleWeaponPickUp(int playerId, Weapon pickedUpWeapon) {
        if (_weaponSpawnCoroutineActive || !pickedUpWeapon.Equals(_currentWeapon)) return;
        StartCoroutine(WeaponSpawnCoroutine());
        _weaponSpawnCoroutineActive = true;
    }

    private IEnumerator WeaponSpawnCoroutine() {
        yield return new WaitForSeconds(_spawnInterval);
        _currentWeapon = Instantiate(_weaponToSpawn, this.transform).GetComponent<Weapon>();
        _weaponSpawnCoroutineActive = false;
    }
}
