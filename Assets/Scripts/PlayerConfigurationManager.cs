using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerConfigurationManager : MonoBehaviour {

    [SerializeField] private PlayerInputManager _playerInputManager;
    private List<PlayerConfiguration> _playerConfigs;
    public static PlayerConfigurationManager Instance { get; private set; }

    private void Awake() {
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(Instance);
            _playerConfigs = new List<PlayerConfiguration>();
        }
    }

    private void OnEnable() {
        _playerInputManager.playerJoinedEvent.AddListener(HandlePlayerJoin);
    }

    private void OnDisable() {
        _playerInputManager?.playerJoinedEvent.RemoveListener(HandlePlayerJoin);
    }

    public void HandlePlayerJoin(PlayerInput playerInput) {
        if (_playerConfigs.Any(player => player.PlayerIndex == playerInput.playerIndex)) return;
        playerInput.transform.SetParent(transform);
        _playerConfigs.Add(new PlayerConfiguration(playerInput));
    }

    public List<PlayerConfiguration> GetPlayerConfigs()
    {
        return _playerConfigs;
    }
    
    public void SetPlayerCharacter(int index, GameObject prefab) {
        _playerConfigs[index].Character = prefab;
    }
    
    public void ReadyPlayer(int index) {
        _playerConfigs[index].IsReady = true;
        if (_playerConfigs.Count >= Settings.MinPlayerAmount && _playerConfigs.All(player => player.IsReady)){
            SceneManager.LoadScene("Testscene");
        }
    }
}

public class PlayerConfiguration {
    public PlayerInput Input { get; set; }
    public int PlayerIndex { get; set; }
    public bool IsReady { get; set; }
    public GameObject Character;
    
    public PlayerConfiguration(PlayerInput playerInput) {
        PlayerIndex = playerInput.playerIndex;
        Input = playerInput;
    }
}