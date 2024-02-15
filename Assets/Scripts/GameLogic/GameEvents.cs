using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

/// <summary>
/// contains all GameEvents methods can be subscribed to
/// the events enable a cleaner communication between classes
/// </summary>
public class GameEvents : MonoBehaviour
{
    private static GameEvents _instance;

    public static GameEvents Instance 
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameEvents>();
            }

            return _instance;
        }
    }

    public UnityEvent<int,Weapon> weaponPickUp;
    public void OnWeaponPickUp(int playerIdToAttachTo, Weapon weaponToAttach) => weaponPickUp?.Invoke(playerIdToAttachTo, weaponToAttach);
    
    public UnityEvent<int> weaponShot;
    public void OnWeaponShot(int attachedPlayerIndex) => weaponShot?.Invoke(attachedPlayerIndex);
    
    public UnityEvent<int> weaponIsEmpty;
    public void OnWeaponIsEmpty(int attachedPlayerIndex) => weaponIsEmpty?.Invoke(attachedPlayerIndex);
    
    public UnityEvent<EntityHealth, float> playerHit;
    public void OnPlayerHit(EntityHealth entityHealth, float damage) => playerHit?.Invoke(entityHealth, damage);
    
    public UnityEvent playerLanded;
    public void OnPlayerLanded() => playerLanded?.Invoke();
    
    public UnityEvent<int> playerDeath;
    public void OnPlayerDeath(int id) => playerDeath?.Invoke(id);

    public UnityEvent roundStart;
    public void OnRoundStart() => roundStart?.Invoke();

    public UnityEvent<int> playerRoundWin;
    public void OnPlayerRoundWin(int id) => playerRoundWin?.Invoke(id);
    
    public UnityEvent<int> playerGameWin;
    public void OnPlayerGameWin(int id) => playerGameWin?.Invoke(id);

    public UnityEvent<bool> pauseGame;
    public void OnPauseGame(bool isPaused) => pauseGame?.Invoke(isPaused);
}