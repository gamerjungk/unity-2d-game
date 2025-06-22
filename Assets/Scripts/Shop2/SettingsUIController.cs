using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUIController : MonoBehaviour
{
    public Slider masterSlider; // 마스터 볼륨 슬라이더
    public Slider bgmSlider;    // BGM 볼륨 슬라이더
    public Slider sfxSlider;    // 효과음 볼륨 슬라이더
    public Toggle muteToggle;   // 음소거 토글
    public TMP_Dropdown languageDropdown;   // 언어 선택 드롭다운

    private void Start()
    {
        // 슬라이더 초기값 반영
        // 초기 슬라이더 및 UI 요소 상태 설정 (저장된 설정 불러옴)
        masterSlider.value = GameSettingsManager.Instance.masterVolume; // 마스터 볼륨 슬라이더 초기화
        bgmSlider.value = GameSettingsManager.Instance.bgmVolume;   // BGM 볼륨 슬라이더 초기화
        sfxSlider.value = GameSettingsManager.Instance.sfxVolume;   // 효과음 볼륨 슬라이더 초기화
        muteToggle.isOn = GameSettingsManager.Instance.isMuted;     // 전체 음소거 토글 설정 (음소거 상태인지 아닌지)

        // 저장된 언어 설정 반영 (0: 한국어, 1: 영어)
        languageDropdown.value = GameSettingsManager.Instance.currentLanguage == SystemLanguage.Korean ? 0 : 1;

        // 슬라이더/토글/드롭다운 UI 요소에 이벤트 리스너 연결
        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged); // 마스터 볼륨 변경 시 실행할 함수 등록
        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);   // BGM 볼륨 변경 시 실행할 함수 등록
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);   // SFX 볼륨 변경 시 실행할 함수 등록
        muteToggle.onValueChanged.AddListener(OnMuteToggle);        // 음소거 토글 변경 시 실행할 함수 등록
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged); // 언어 선택 변경 시 실행할 함수 등록
    }


    // 마스터 볼륨 변경 처리
    public void OnMasterVolumeChanged(float value)
    {
        int previousMaster = GameSettingsManager.Instance.masterVolume; // 기존 마스터 볼륨 값 저장
        int newMaster = Mathf.RoundToInt(value);                        // 새 입력값을 정수로 반올림

        // 0으로 나누는 상황 방지
        if (previousMaster == 0)
            previousMaster = 1; 

        float ratio = newMaster / (float)previousMaster;  // 볼륨 비율 계산

        // 기존 BGM/SFX 볼륨에 비율 적용하여 조절 (1~100 범위 제한)
        GameSettingsManager.Instance.bgmVolume = Mathf.Clamp(
            Mathf.RoundToInt(GameSettingsManager.Instance.bgmVolume * ratio), 1, 100);
        GameSettingsManager.Instance.sfxVolume = Mathf.Clamp(
            Mathf.RoundToInt(GameSettingsManager.Instance.sfxVolume * ratio), 1, 100);

        GameSettingsManager.Instance.masterVolume = newMaster;  // 마스터 볼륨 설정 갱신
        GameSettingsManager.Instance.ApplyAudioSettings();      // 실제 오디오 시스템에 적용
        GameSettingsManager.Instance.SaveSettings();            // 변경된 설정 저장

        // UI 슬라이더에도 반영 (눈에 보이는 값도 동기화)
        bgmSlider.value = GameSettingsManager.Instance.bgmVolume;   
        sfxSlider.value = GameSettingsManager.Instance.sfxVolume;
    }

    // BGM 볼륨 변경 처리
    public void OnBGMVolumeChanged(float value)
    {
        GameSettingsManager.Instance.bgmVolume = Mathf.RoundToInt(value);   // 값 반올림 후 저장
        GameSettingsManager.Instance.ApplyAudioSettings();                  // 실제 오디오에 적용
        GameSettingsManager.Instance.SaveSettings();                        // 설정 저장
    }

    // SFX 볼륨 변경 처리
    public void OnSFXVolumeChanged(float value)
    {
        GameSettingsManager.Instance.sfxVolume = Mathf.RoundToInt(value);   // 값 반올림 후 저장
        GameSettingsManager.Instance.ApplyAudioSettings();                  // 실제 오디오에 적용
        GameSettingsManager.Instance.SaveSettings();                        // 설정 저장
    }

    
    public void OnMuteToggle(bool isMuted)
    {
        GameSettingsManager.Instance.isMuted = isMuted;                     // 음소거 상태 저장
        GameSettingsManager.Instance.ApplyAudioSettings();                  // 실제 오디오에 적용
        GameSettingsManager.Instance.SaveSettings();                        // 설정 저장
    }

    public void OnLanguageChanged(int index)
    {
        var lang = index == 0 ? SystemLanguage.Korean : SystemLanguage.English; // 선택된 인덱스를 언어로 변환
        GameSettingsManager.Instance.SetLanguage(lang);                         // GameSettings에 언어 설정

       
        LocalizationManager.Instance.ChangeLanguage(index == 0 ? Language.Korean : Language.English); // LocalizationManager에도 언어 변경 요청 (UI 텍스트 실시간 변경 등)
    }
}
