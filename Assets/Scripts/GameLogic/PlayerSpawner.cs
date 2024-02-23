using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour { 
    private List<PlayerConfiguration> _playerConfigurations;
    [SerializeField] private List<Transform> _spawnTransforms;

    private void Awake() {
        _playerConfigurations= PlayerConfigurationManager.Instance.GetPlayerConfigs();
        for (int i = 0; i < _playerConfigurations.Count; i++){
            _spawnTransforms[i].gameObject.SetActive(true);
            switch (i){
                
            }
            _spawnTransforms[i].gameObject.GetComponent<Material>().color = new Color();
        }
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
            spawnedPlayer.GetComponent<PlayerController>().InitializePlayer(_playerConfigurations[i]);
            spawnedPlayer.GetComponent<EntityHealth>().PlayerIndex = _playerConfigurations[i].PlayerIndex;
            
            _spawnParticles[i].Play();
        }
    }
}