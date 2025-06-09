using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleUIController : MonoBehaviour
{
    public Slider masterSlider;
    public Toggle muteToggle;

    [Header("🔊 사운드 아이콘 설정")]
    public Image soundIconImage;
    public Sprite soundOnIcon;
    public Sprite soundOffIcon;

    private void Start()
    {
        masterSlider.value = GameSettingsManager.Instance.masterVolume;
        muteToggle.isOn = GameSettingsManager.Instance.isMuted;

        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        muteToggle.onValueChanged.AddListener(OnMuteToggle);

        UpdateSoundIcon(); // 초기 상태 반영
    }

    public void OnMasterVolumeChanged(float value)
    {
        // 🔧 값이 너무 작으면 0으로 간주
        if (value <= 0.01f)
        {
            value = 0f;
            masterSlider.SetValueWithoutNotify(0f);
        }

        int newMaster = Mathf.RoundToInt(value);
        int previousMaster = GameSettingsManager.Instance.masterVolume;

        if (newMaster != previousMaster)
        {
            float ratio = (previousMaster == 0) ? 1f : newMaster / (float)previousMaster;

            GameSettingsManager.Instance.masterVolume = newMaster;

            // ✅ bgm/sfx가 0일 경우는 기본값으로 복구, 그렇지 않으면 비율로 조정
            int currentBGM = GameSettingsManager.Instance.bgmVolume;
            int currentSFX = GameSettingsManager.Instance.sfxVolume;

            GameSettingsManager.Instance.bgmVolume = (currentBGM == 0)
                ? 100
                : Mathf.Clamp(Mathf.RoundToInt(currentBGM * ratio), 0, 100);

            GameSettingsManager.Instance.sfxVolume = (currentSFX == 0)
                ? 100
                : Mathf.Clamp(Mathf.RoundToInt(currentSFX * ratio), 0, 100);
        }

        // 🔇 자동 음소거 처리
        bool shouldBeMuted = (newMaster == 0);
        if (GameSettingsManager.Instance.isMuted != shouldBeMuted)
        {
            GameSettingsManager.Instance.isMuted = shouldBeMuted;
            muteToggle.SetIsOnWithoutNotify(shouldBeMuted);
        }

        GameSettingsManager.Instance.ApplyAudioSettings();
        GameSettingsManager.Instance.SaveSettings();

        UpdateSoundIcon();
    }

    public void OnMuteToggle(bool isMuted)
    {
        GameSettingsManager.Instance.isMuted = isMuted;

        // 🔊 음소거 해제 후 마스터 볼륨이 0이면 복원
        if (!isMuted && GameSettingsManager.Instance.masterVolume == 0)
        {
            GameSettingsManager.Instance.masterVolume = 1;
            masterSlider.SetValueWithoutNotify(1);
        }

        GameSettingsManager.Instance.ApplyAudioSettings();
        GameSettingsManager.Instance.SaveSettings();

        UpdateSoundIcon();
    }

    private void UpdateSoundIcon()
    {
        Debug.Log("🔄 사운드 아이콘 갱신됨");

        soundIconImage.sprite =
            (GameSettingsManager.Instance.isMuted || GameSettingsManager.Instance.masterVolume == 0)
            ? soundOffIcon
            : soundOnIcon;
    }
}
