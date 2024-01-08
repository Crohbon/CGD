using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

    [SerializeField] private Button _initialSelectedButton;
    [SerializeField] private Button _mainMenuSettingsButton;
    
    [SerializeField] private TMP_Text _winPointsHandicapText;
    [SerializeField] private TMP_Text _winPointsTotalText;
    [SerializeField] private TMP_Text _musicVolumeText;
    [SerializeField] private TMP_Text _sfxVolumeText;

    private void OnEnable() {
        _initialSelectedButton.Select();
        UpdateWinPointTexts();
        UpdateSfxVolumeText();
        UpdateMusicVolumeText();
    }

    private void OnDisable() {
        _mainMenuSettingsButton.Select();
    }

    public void IncreaseWinPointsNeeded() {
        Settings.PointsPerHandicap++;

        UpdateWinPointTexts();
    }
    
    public void DecreaseWinPointsNeeded() {
        if (Settings.PointsPerHandicap <= 1) return; 
        Settings.PointsPerHandicap--;

        UpdateWinPointTexts();
    }

    public void IncreaseMusicVolume() {
        if (Settings.MusicVolume >= 1f){
            Settings.MusicVolume = 1f;
            return;
        }
        Settings.MusicVolume += 0.05f;

        UpdateMusicVolumeText();
    }
    
    public void DecreaseMusicVolume() {
        if (Settings.MusicVolume <= 0f){
            Settings.MusicVolume = 0f;
            return;
        }
        Settings.MusicVolume -= 0.05f;

        UpdateMusicVolumeText();
    }
    
    public void IncreaseSfxVolume() {
        if (Settings.SfxVolume >= 1f){
            Settings.SfxVolume = 1f;
            return;
        }
        Settings.SfxVolume += 0.05f;

        UpdateSfxVolumeText();
    }
    
    public void DecreaseSfxVolume() {
        if (Settings.SfxVolume <= 0f){
            Settings.SfxVolume = 0f;
            return;
        }
        Settings.SfxVolume -= 0.05f;
        
        UpdateSfxVolumeText();
    }

    private void UpdateWinPointTexts() {
        _winPointsHandicapText.text = Settings.PointsPerHandicap.ToString();
        _winPointsTotalText.text = (Settings.PointsPerHandicap * Settings.TotalHandicapTiers).ToString();
    }
    
    private void UpdateMusicVolumeText() {
        _musicVolumeText.text = ((int)(Settings.MusicVolume * 100)).ToString();
    }
    
    private void UpdateSfxVolumeText() {
        _sfxVolumeText.text = ((int)(Settings.SfxVolume * 100)).ToString();
    }
}