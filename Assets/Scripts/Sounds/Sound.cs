using UnityEngine;

public class Sound : MonoBehaviour
{
    [SerializeField] private AudioClip[] playerHitSounds; //Sound hinzufügen

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            // Wenn Audio nicht automatisch gefunden wird, füge es hinzu
            audioSource = gameObject.AddComponent<AudioSource>();
        }

       
        GameEvents.Instance.playerHit.AddListener(PlayRandomPlayerHitSound);
    }

    private void OnDestroy()
    {
        
        if (GameEvents.Instance != null && GameEvents.Instance.playerHit != null)
        {
            GameEvents.Instance.playerHit.RemoveListener(PlayRandomPlayerHitSound);
        }
    }

    private void PlayRandomPlayerHitSound(EntityHealth entityHealth, float damage)
    {
        // Überprüfen, ob das getroffene GameObject ein Spieler ist
        if (entityHealth != null && entityHealth.CompareTag("Player"))
        {
            // Zufälliger Sound
            if (playerHitSounds != null && playerHitSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, playerHitSounds.Length);
                AudioClip randomHitSound = playerHitSounds[randomIndex];

                // Überprüfen, ob der AudioClip null ist
                if (randomHitSound != null)
                {
                    // Überprüfen, ob die AudioSource aktiviert ist, bevor der Sound abgespielt wird
                    if (!audioSource.enabled)
                    {
                        audioSource.enabled = true;
                    }

                    audioSource.PlayOneShot(randomHitSound);
                }
            }
        }
    }
}
