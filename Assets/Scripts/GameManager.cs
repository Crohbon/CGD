using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private List<int> _winPoints;
    private List<bool> _alivePlayers;

    private int _playerAmount;
    private Coroutine _roundStartCoroutine;

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

        if (_alivePlayers.Count(playerAlive => playerAlive) > 1) return;

        int lastPlayerIndex = _alivePlayers.IndexOf(true);
        _winPoints[lastPlayerIndex]++;

        if (_winPoints.Any(playerPoints => playerPoints >= Settings.PointsPerHandicap * Settings.TotalHandicapTiers)){
            GameEvents.Instance.OnPlayerGameWin(lastPlayerIndex);
        }
        else{
            GameEvents.Instance.OnPlayerRoundWin(lastPlayerIndex);
            StartRound();
        }
        
    }

    #endregion

    private void InitializeGame() {
        _playerAmount = PlayerConfigurationManager.Instance.GetPlayerConfigs().Count;
        _winPoints = new List<int>();
        for (int i = 0; i < _playerAmount; i++){
            _winPoints.Add(0);
        }
        StartRound();
    }

    private void StartRound() {
        _alivePlayers = new List<bool>();
        for (int i = 0; i < _playerAmount; i++){
            _alivePlayers.Add(true);
        }

        if (_roundStartCoroutine != null){
            StopCoroutine(_roundStartCoroutine);
        }
        _roundStartCoroutine = StartCoroutine(RoundStartCoroutine());
    }

    private IEnumerator RoundStartCoroutine() {
        yield return new WaitForSeconds(3f);
        GameEvents.Instance.OnRoundStart();
    }
}