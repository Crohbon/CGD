using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private Transform _scoreParent;
    [SerializeField] private GameObject _scorePrefab;
    private List<TextMeshProUGUI> _scoreTexts;
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
            _scoreTexts[lastPlayerIndex].SetText("Player " + (lastPlayerIndex + 1) + ": " + _winPoints[lastPlayerIndex]);
            StartRound();
        }
        
    }

    #endregion

    private void InitializeGame() {
        _playerAmount = PlayerConfigurationManager.Instance.GetPlayerConfigs().Count;
        _winPoints = new List<int>();
        _scoreTexts = new List<TextMeshProUGUI>();
        for (int i = 0; i < _playerAmount; i++){
            _winPoints.Add(0);
            GameObject scoreEntry = Instantiate(_scorePrefab, _scoreParent);
            _scoreTexts.Add(scoreEntry.GetComponentInChildren<TextMeshProUGUI>());
            _scoreTexts[i].SetText("Player " + (i+1) + ": 0");
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