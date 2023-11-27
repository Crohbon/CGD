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

    public UnityEvent<int> weaponPickUp;
    public void OnWeaponPickUp(int phase) => weaponPickUp?.Invoke(phase);
    
    public UnityEvent<int> weaponIsEmpty;
    public void OnWeaponIsEmpty(int phase) => weaponIsEmpty?.Invoke(phase);

}