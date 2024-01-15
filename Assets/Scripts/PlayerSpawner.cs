using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour { 
    private List<PlayerConfiguration> _playerConfigurations;
    [SerializeField] private List<Transform> _spawnTransforms;

    private void Awake() {
        _playerConfigurations= PlayerConfigurationManager.Instance.GetPlayerConfigs();
    }

    private void OnEnable() {
        GameEvents.Instance.roundStart.AddListener(HandleRoundStart);
    }

    private void OnDisable() {
        GameEvents.Instance?.roundStart.RemoveListener(HandleRoundStart);
    }

    private void HandleRoundStart() {
        for (int i = 0; i < transform.childCount; i++){
            Destroy(transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _playerConfigurations.Count; i++){
            GameObject spawnedPlayer = Instantiate(_playerConfigurations[i].Character, _spawnTransforms[i].position, _spawnTransforms[i].rotation, transform);
            spawnedPlayer.GetComponent<PlayerController>().InitializeControls(_playerConfigurations[i]);
        }
    }
}