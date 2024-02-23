using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour { 
    private List<PlayerConfiguration> _playerConfigurations;
    [SerializeField] private List<Transform> _spawnTransforms;

    private void Awake() {
        _playerConfigurations= PlayerConfigurationManager.Instance.GetPlayerConfigs();
        for (int i = 0; i < _playerConfigurations.Count; i++){
            _spawnTransforms[i].gameObject.SetActive(true);
            
            ParticleSystem particleSystem = _spawnTransforms[i].gameObject.GetComponent<ParticleSystem>(); 
            
            switch (_playerConfigurations[i].Character.name){
                case "Player_1":
                    particleSystem.startColor = new Color(0.188f, 0.329f, 0.451f);
                    break;
                case "Player_2":
                    particleSystem.startColor = new Color(0.294f, 0.388f, 0.172f);
                    break;
                case "Player_3":
                    particleSystem.startColor = new Color(0.639f, 0.341f, 0.105f);
                    break;
                case "Player_4":
                    particleSystem.startColor = new Color(0.768f, 0.615f, 0.247f);
                    break;
            }
            particleSystem.Play();
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
        }
    }
}