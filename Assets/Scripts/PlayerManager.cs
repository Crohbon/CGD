using TarodevController;
using Unity.Mathematics;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    [SerializeField] private GameObject playerOnePrefab;
    [SerializeField] private GameObject playerTwoPrefab;
    private GameObject _playerOne;
    private Transform _spawnTransform;
    
    private void Start() {
        _spawnTransform = transform;
        
        _playerOne = Instantiate(playerOnePrefab, _spawnTransform.position,_spawnTransform.rotation);
        Instantiate(playerTwoPrefab, _spawnTransform.position, _spawnTransform.rotation);
    }
}