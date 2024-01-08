using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class SpawnPlayerSetupMenu : MonoBehaviour {
    [SerializeField] private GameObject _playerSetupMenuPrefab;
    [SerializeField] private PlayerInput _playerInput;
    private GameObject _rootMenu;

    private void Awake() {
        _rootMenu = GameObject.Find("PlayerSelectionLayout");
        if (_rootMenu == null) return;
        GameObject menu = Instantiate(_playerSetupMenuPrefab, _rootMenu.transform);
        _playerInput.uiInputModule = menu.GetComponentInChildren<InputSystemUIInputModule>();
        menu.GetComponent<PlayerSetupMenuController>().SetPlayerIndex(_playerInput.playerIndex);

    }
}