using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {
    
    public static GameManager Instance { get; private set; }
    
    public  List<int> WinPoints { get; private set; }
    private List<bool> _alivePlayers;

    private int _playerAmount;
    private Coroutine _roundStartCoroutine;

    private void Awake() {
        if (Instance == null){
            Instance = this;
        }

        InitializeGame();
    }

    #region Events

    private void OnEnable() {
        GameEvents.Instance.playerDeath.AddListener(HandlePlayerDeath);
        GameEvents.Instance.pauseGame.AddListener(HandlePauseGame);
    }

    private void OnDisable() {
        GameEvents.Instance?.playerDeath.RemoveListener(HandlePlayerDeath);
        GameEvents.Instance?.pauseGame.RemoveListener(HandlePauseGame);
    }

    private void HandlePlayerDeath(int playerIndex) {
        _alivePlayers[playerIndex] = false;

        if (_alivePlayers.Count(playerAlive => playerAlive) != 1){
            Logger.LogGameState(false, false, _alivePlayers, WinPoints);
            return;
        }

        int lastPlayerIndex = _alivePlayers.IndexOf(true);
        WinPoints[lastPlayerIndex]++;

        if (WinPoints.Any(playerPoints => playerPoints >= Settings.PointsPerHandicap * Settings.TotalHandicapTiers)){
            GameEvents.Instance.OnPlayerGameWin(lastPlayerIndex);
            Logger.LogGameState(false, true, _alivePlayers, WinPoints);
        }
        else{
            GameEvents.Instance.OnPlayerRoundWin(lastPlayerIndex);
            Logger.LogGameState(true, false, _alivePlayers, WinPoints);
            StartRound();
        }
    }

    #endregion

    private void InitializeGame() {
        _playerAmount = PlayerConfigurationManager.Instance.GetPlayerConfigs().Count;
        WinPoints = new List<int>();
        for (int i = 0; i < _playerAmount; i++){
            WinPoints.Add(0);
        }
        StartRound();
    }

    private void StartRound() {
        if (_roundStartCoroutine != null){
            StopCoroutine(_roundStartCoroutine);
        }
        _roundStartCoroutine = StartCoroutine(RoundStartCoroutine());
    }

    private void HandlePauseGame(bool pauseState) {
        Time.timeScale = pauseState ? 0 : 1;
    }

    private IEnumerator RoundStartCoroutine() {
        yield return new WaitForSeconds(3f);
        
        _alivePlayers = new List<bool>();
        for (int i = 0; i < _playerAmount; i++){
            _alivePlayers.Add(true);
        }
        GameEvents.Instance.OnRoundStart();
    }
}