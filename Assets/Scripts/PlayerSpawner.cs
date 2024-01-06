using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour {
    [SerializeField] private List<GameObject> playerPrefabs;
    private List<Transform> _spawnTransforms;

    private void OnEnable() {
        GameEvents.Instance.roundStart.AddListener(HandleRoundStart);
    }

    private void OnDisable() {
        GameEvents.Instance?.roundStart.RemoveListener(HandleRoundStart);
    }

    private void HandleRoundStart() {
        for (int i = 0; i < Settings.PlayerAmount; i++){
            Instantiate(playerPrefabs[i], _spawnTransforms[i].position, Quaternion.identity);
        }
    }
}