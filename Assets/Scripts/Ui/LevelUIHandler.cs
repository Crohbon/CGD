using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelUIHandler : MonoBehaviour {
    [SerializeField] private Transform _roundEndScreen;
    [SerializeField] private TextMeshProUGUI _roundWinText;
    
    [SerializeField] private Transform _handicapRoot;
    [SerializeField] private Image _handicapImage;
    [SerializeField] private List<Sprite> _handicapSprites;
    [SerializeField] private TextMeshProUGUI _handicapText;

    [SerializeField] private Transform _gameEndScreen;
    [SerializeField] private TextMeshProUGUI _gameWinText;
    [SerializeField] private Button _mainMenuButton;

    private void Awake() {
        _mainMenuButton.onClick.AddListener(LoadMainMenu);
    }

    private void OnEnable() {
        GameEvents.Instance.playerRoundWin.AddListener(HandlePlayerRoundWin);
        GameEvents.Instance.roundStart.AddListener(HandleRoundStart);
        GameEvents.Instance.playerGameWin.AddListener(HandlePlayerGameWin);
    }

    private void OnDisable() {
        GameEvents.Instance?.playerRoundWin.RemoveListener(HandlePlayerRoundWin);
        GameEvents.Instance?.roundStart.RemoveListener(HandleRoundStart);
        GameEvents.Instance?.playerGameWin.RemoveListener(HandlePlayerGameWin);
    }

    private void HandlePlayerRoundWin(int playerIndex) {
        _roundEndScreen.gameObject.SetActive(true);
        _roundWinText.SetText("Player " + (playerIndex + 1) + " won this round");
        
        int winningPlayerPoints = GameManager.Instance.WinPoints[playerIndex];

        if (winningPlayerPoints % Settings.PointsPerHandicap != 0) return;
        
        _handicapRoot.gameObject.SetActive(true);
        switch (winningPlayerPoints/Settings.PointsPerHandicap){
            case 1:
                _handicapText.SetText("Less Damage");
                _handicapImage.sprite = _handicapSprites[0];
                break;
            case 2:
                _handicapText.SetText("Bullet Drop");
                _handicapImage.sprite = _handicapSprites[1];
                break;
            case 3:
                _handicapText.SetText("Hitbox Increase");
                _handicapImage.sprite = _handicapSprites[2];
                break;
            case 4:
                _handicapText.SetText("Controls Invert");
                _handicapImage.sprite = _handicapSprites[3];
                break;
        }
    }

    private void HandleRoundStart() {
        _roundEndScreen.gameObject.SetActive(false);
        _handicapRoot.gameObject.SetActive(false);
    }

    private void HandlePlayerGameWin(int playerIndex) {
        Destroy(PlayerConfigurationManager.Instance.gameObject);
        _gameEndScreen.gameObject.SetActive(true);
        _gameWinText.SetText("Player " + (playerIndex + 1) + " won the turf war");
        _mainMenuButton.Select();
    }
    
    private void LoadMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}
