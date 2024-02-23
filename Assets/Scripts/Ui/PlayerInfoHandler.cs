using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoHandler : MonoBehaviour {
    [SerializeField] private GameObject _visuals;
    [SerializeField] private List<GameObject> _handicapVisuals;
    [SerializeField] private int _playerIndex;
    [SerializeField] private TextMeshProUGUI _playerAmmunitionText;
    [SerializeField] private TextMeshProUGUI _playerWinPointsText;
    [SerializeField] private Slider _healthBar;

    private int _remainingAmmunition;
    private int _winPointAmount = 0;

    private void Awake() {
        if (PlayerConfigurationManager.Instance.GetPlayerConfigs().Count > _playerIndex){
            _visuals.SetActive(true);
        }
        else{
            gameObject.SetActive(false);
        }
    }

    private void OnEnable() {
        GameEvents.Instance.playerRoundWin.AddListener(HandlePlayerRoundWin);
        GameEvents.Instance.roundStart.AddListener(HandleRoundStart);
        GameEvents.Instance.weaponPickUp.AddListener(HandleWeaponPickUp);
        GameEvents.Instance.playerHit.AddListener(HandlePlayerHit);
        GameEvents.Instance.weaponShot.AddListener(HandelWeaponShot);
    }

    private void OnDisable() {
        GameEvents.Instance?.playerRoundWin.RemoveListener(HandlePlayerRoundWin);
        GameEvents.Instance?.roundStart.RemoveListener(HandleRoundStart);
        GameEvents.Instance?.weaponPickUp.RemoveListener(HandleWeaponPickUp);
        GameEvents.Instance?.playerHit.RemoveListener(HandlePlayerHit);
        GameEvents.Instance?.weaponShot.RemoveListener(HandelWeaponShot);
    }
    
    private void HandlePlayerRoundWin(int lastPlayerIndex) {
        if (lastPlayerIndex != _playerIndex) return;
        
        _winPointAmount++;
        _playerWinPointsText.SetText("" + _winPointAmount);
    }

    private void HandleRoundStart() {
        _healthBar.SetValueWithoutNotify(0f);
        
        switch (GameManager.Instance.WinPoints[_playerIndex]/Settings.PointsPerHandicap){
            case 1:
                _handicapVisuals[0].SetActive(true);
                break;
            case 2:
                _handicapVisuals[1].SetActive(true);
                break;
            case 3:
                _handicapVisuals[2].SetActive(true);
                break;
            case 4:
                _handicapVisuals[0].SetActive(false);
                _handicapVisuals[1].SetActive(false);
                _handicapVisuals[2].SetActive(false);
                _handicapVisuals[3].SetActive(true);
                break;
        }
    }

    private void HandleWeaponPickUp(int index, Weapon weaponToAttach) {
        if (index != _playerIndex) return;

        _remainingAmmunition = weaponToAttach.GetAmmunition();
        _playerAmmunitionText.SetText("" + _remainingAmmunition);
    }
    
    private void HandlePlayerHit(EntityHealth entityHealth, float damage) {
        if (entityHealth.PlayerIndex != _playerIndex) return;

        _healthBar.SetValueWithoutNotify(Mathf.Min(1, 1 - ((entityHealth.CurrentHealth - damage) / entityHealth.BaseHealth)));
    }
    
    private void HandelWeaponShot(int attachedPlayerIndex) {
        if (attachedPlayerIndex != _playerIndex) return;

        _remainingAmmunition--;
        _playerAmmunitionText.SetText("" + _remainingAmmunition);
    }
}
