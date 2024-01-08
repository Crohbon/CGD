using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetupMenuController : MonoBehaviour {
    private int _playerIndex;

    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private GameObject _readyPanel;
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private Button _readyButton;

    private float _ignoreInputTime = 1.5f;
    private bool _inputEnabled;

    private void Update() {
        if (Time.time > _ignoreInputTime){
            _inputEnabled = true;
        }
    }
    
    public void SetPlayerIndex(int playerIndex) {
        _playerIndex = playerIndex;
        _titleText.SetText("Player " + (playerIndex + 1).ToString());
        _ignoreInputTime += Time.time;
    }

    public void SetPlayerCharacter(GameObject prefab) {
        if (!_inputEnabled) return;
        PlayerConfigurationManager.Instance.SetPlayerCharacter(_playerIndex, prefab);
        _readyPanel.SetActive(true);
        _readyButton.Select();
        _menuPanel.SetActive(false);
    }

    public void ReadyPlayer() {
        if (!_inputEnabled) return;
        PlayerConfigurationManager.Instance.ReadyPlayer(_playerIndex);
        _readyButton.gameObject.SetActive(false);
    }
}
