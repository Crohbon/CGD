using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameState : MonoBehaviour{
    public List<int> _winPoints;
    private List<bool> _alivePlayers;
    private List<bool> _playerSlotFilled;

    private void Awake() {
        InitializeGame();
    }

    #region Events

    private void OnEnable() {
        GameEvents.Instance.playerDeath.AddListener(HandlePlayerDeath);
    }

    private void OnDisable() {
        GameEvents.Instance?.playerDeath.RemoveListener(HandlePlayerDeath);
    }

    private void HandlePlayerDeath(int playerIndex) {
        _alivePlayers[playerIndex] = false;

        int playersAlive = _alivePlayers.Count(playerAlive => playerAlive);

        if (playersAlive > 1) return;
        Time.timeScale = 0;
        
        int lastPlayerIndex = _alivePlayers.IndexOf(true);
        _winPoints[lastPlayerIndex]++;

        if (_winPoints.Any(playerPoints => playerPoints >= Settings.PointsPerHandicap * Settings.TotalHandicapTiers)){
            GameEvents.Instance.OnPlayerGameWin(lastPlayerIndex);
        }
        else{
            GameEvents.Instance.OnPlayerRoundWin(lastPlayerIndex);
        }
    }

    #endregion

    private void InitializeGame() {
        _winPoints = new List<int>(Settings.PlayerAmount);
        _playerSlotFilled = new List<bool>(Settings.MaxPlayerAmount);
        for (int i = 0; i < Settings.PlayerAmount; i++){
            _playerSlotFilled[i] = true;
        }
        StartRound();
    }

    private void StartRound() {
        _alivePlayers = _playerSlotFilled;
        GameEvents.Instance.OnRoundStart();
        StartCoroutine(RoundStartCoroutine());
    }

    private IEnumerator RoundStartCoroutine() {
        yield return new WaitForSeconds(3f);
        Time.timeScale = 1;
    }
}