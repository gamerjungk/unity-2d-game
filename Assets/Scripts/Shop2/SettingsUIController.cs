using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUIController : MonoBehaviour
{
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Toggle muteToggle;
    public TMP_Dropdown languageDropdown;

    private void Start()
    {
        // 슬라이더 초기값 반영
        masterSlider.value = GameSettingsManager.Instance.masterVolume;
        bgmSlider.value = GameSettingsManager.Instance.bgmVolume;
        sfxSlider.value = GameSettingsManager.Instance.sfxVolume;
        muteToggle.isOn = GameSettingsManager.Instance.isMuted;
        languageDropdown.value = GameSettingsManager.Instance.currentLanguage == SystemLanguage.Korean ? 0 : 1;

        // 리스너 연결
        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        muteToggle.onValueChanged.AddListener(OnMuteToggle);
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    public void OnMasterVolumeChanged(float value)
    {
        int previousMaster = GameSettingsManager.Instance.masterVolume;
        int newMaster = Mathf.RoundToInt(value);

        if (previousMaster == 0)
            previousMaster = 1; // 0으로 나누기 방지

        float ratio = newMaster / (float)previousMaster;

        // 🔁 bgm/sfx 값을 비율로 조정하되 상한선 100 유지
        GameSettingsManager.Instance.bgmVolume = Mathf.Clamp(
            Mathf.RoundToInt(GameSettingsManager.Instance.bgmVolume * ratio), 1, 100);
        GameSettingsManager.Instance.sfxVolume = Mathf.Clamp(
            Mathf.RoundToInt(GameSettingsManager.Instance.sfxVolume * ratio), 1, 100);

        GameSettingsManager.Instance.masterVolume = newMaster;
        GameSettingsManager.Instance.ApplyAudioSettings();
        GameSettingsManager.Instance.SaveSettings();

        // ✅ 슬라이더 UI 반영
        bgmSlider.value = GameSettingsManager.Instance.bgmVolume;
        sfxSlider.value = GameSettingsManager.Instance.sfxVolume;
    }


    public void OnBGMVolumeChanged(float value)
    {
        GameSettingsManager.Instance.bgmVolume = Mathf.RoundToInt(value);
        GameSettingsManager.Instance.ApplyAudioSettings();
        GameSettingsManager.Instance.SaveSettings();
    }

    public void OnSFXVolumeChanged(float value)
    {
        GameSettingsManager.Instance.sfxVolume = Mathf.RoundToInt(value);
        GameSettingsManager.Instance.ApplyAudioSettings();
        GameSettingsManager.Instance.SaveSettings();
    }

    public void OnMuteToggle(bool isMuted)
    {
        GameSettingsManager.Instance.isMuted = isMuted;
        GameSettingsManager.Instance.ApplyAudioSettings();
        GameSettingsManager.Instance.SaveSettings();
    }

    public void OnLanguageChanged(int index)
    {
        var lang = index == 0 ? SystemLanguage.Korean : SystemLanguage.English;
        GameSettingsManager.Instance.SetLanguage(lang);

        LocalizationManager.Instance.ChangeLanguage(index == 0 ? Language.Korean : Language.English);
    }
}
