using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 간단한 설정 UI를 제어하는 클래스 (마스터 볼륨 + 음소거)
public class SimpleUIController : MonoBehaviour
{
    public Slider masterSlider; // 마스터 볼륨 조절 슬라이더
    public Toggle muteToggle;   // 음소거 토글

    [Header("🔊 사운드 아이콘 설정")]
    public Image soundIconImage;    // 현재 사운드 상태를 나타내는 이미지
    public Sprite soundOnIcon;      // 사운드 켜짐 상태 아이콘
    public Sprite soundOffIcon;     // 사운드 꺼짐 상태 아이콘

    private void Start()
    {   
        // 슬라이더 및 토글의 초기값을 GameSettingsManager에서 불러와 반영
        masterSlider.value = GameSettingsManager.Instance.masterVolume;
        muteToggle.isOn = GameSettingsManager.Instance.isMuted;

        // 슬라이더/토글 값 변경 시 실행될 리스너 등록
        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        muteToggle.onValueChanged.AddListener(OnMuteToggle);

        UpdateSoundIcon(); // 사운드 상태 아이콘 초기화
    }

    // 마스터 볼륨이 변경되었을 때 실행되는 함수
    public void OnMasterVolumeChanged(float value)
    {
        // 너무 작은 값은 0으로 간주하여 처리 (정확한 0 인식)
        if (value <= 0.01f)
        {
            value = 0f;
            masterSlider.SetValueWithoutNotify(0f); // 이벤트 재발생 없이 슬라이더 값 설정
        }

        int newMaster = Mathf.RoundToInt(value);    // 입력값 정수화
        int previousMaster = GameSettingsManager.Instance.masterVolume; // 이전 볼륨 값 저장

        if (newMaster != previousMaster)
        {
            float ratio = (previousMaster == 0) ? 1f : newMaster / (float)previousMaster;

            GameSettingsManager.Instance.masterVolume = newMaster;  // 마스터 볼륨 저장

            // bgm/sfx가 0일 경우는 기본값으로 복구, 그렇지 않으면 비율로 조정
            int currentBGM = GameSettingsManager.Instance.bgmVolume;    // 기존 BGM 볼륨
            int currentSFX = GameSettingsManager.Instance.sfxVolume;    // 기존 SFX 볼륨

            GameSettingsManager.Instance.bgmVolume = (currentBGM == 0)
                ? 100
                : Mathf.Clamp(Mathf.RoundToInt(currentBGM * ratio), 0, 100);

            GameSettingsManager.Instance.sfxVolume = (currentSFX == 0)
                ? 100
                : Mathf.Clamp(Mathf.RoundToInt(currentSFX * ratio), 0, 100);
        }

        // 마스터 볼륨 0이면 자동으로 음소거 토글 ON, 아닐 경우 OFF
        bool shouldBeMuted = (newMaster == 0);
        if (GameSettingsManager.Instance.isMuted != shouldBeMuted)
        {
            GameSettingsManager.Instance.isMuted = shouldBeMuted;
            muteToggle.SetIsOnWithoutNotify(shouldBeMuted); // 토글 상태 UI 갱신
        }

        GameSettingsManager.Instance.ApplyAudioSettings();  // 오디오 설정 적용
        GameSettingsManager.Instance.SaveSettings();        // 설정 저장

        UpdateSoundIcon();  // 아이콘 상태 갱신
    }

    // 음소거 토글이 변경되었을 때 실행되는 함수
    public void OnMuteToggle(bool isMuted)
    {
        GameSettingsManager.Instance.isMuted = isMuted; // 상태 저장

        // 음소거 해제 시 마스터 볼륨이 0이면 최소값 1로 복원
        if (!isMuted && GameSettingsManager.Instance.masterVolume == 0)
        {
            GameSettingsManager.Instance.masterVolume = 1;  
            masterSlider.SetValueWithoutNotify(1);
        }
        
        GameSettingsManager.Instance.ApplyAudioSettings();  // 오디오 설정 적용
        GameSettingsManager.Instance.SaveSettings();    // 설정 저장

        UpdateSoundIcon();
    }

    // 사운드 상태에 따라 아이콘을 갱신하는 함수
    private void UpdateSoundIcon()
    {
        Debug.Log("사운드 아이콘 갱신됨");

        // 음소거거나 마스터 볼륨이 0이면 꺼진 아이콘, 아니면 켜진 아이콘
        soundIconImage.sprite =
            (GameSettingsManager.Instance.isMuted || GameSettingsManager.Instance.masterVolume == 0)
            ? soundOffIcon
            : soundOnIcon;
    }
}
