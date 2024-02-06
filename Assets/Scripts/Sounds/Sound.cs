using UnityEngine;
using Random = UnityEngine.Random;

public class Sound : MonoBehaviour {
    [SerializeField] private AudioClip[] playerHitSounds; //Sound hinzufügen
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private EntityHealth _playerHealth;
    private int _playerIndex = 0;

    private void Start() {
        _playerIndex = _playerHealth.PlayerIndex;
    }

    private void OnEnable() {
        GameEvents.Instance.playerHit.AddListener(PlayRandomPlayerHitSound);
    }

    private void OnDisable() {
        GameEvents.Instance?.playerHit.RemoveListener(PlayRandomPlayerHitSound);
    }

    private void PlayRandomPlayerHitSound(EntityHealth entityHealth, float damage) {
        
        // Überprüfen, ob das getroffene GameObject ein Spieler ist
        if (_playerIndex != entityHealth.PlayerIndex) return;
        int randomIndex = Random.Range(0, playerHitSounds.Length);
        AudioClip randomHitSound = playerHitSounds[randomIndex];
        
        // Überprüfen, ob der AudioClip null ist
        if (randomHitSound == null) return;
        
        // Überprüfen, ob die AudioSource aktiviert ist, bevor der Sound abgespielt wird
        if (!_audioSource.enabled){ 
            _audioSource.enabled = true;
        }
        _audioSource.PlayOneShot(randomHitSound);
    }
}

