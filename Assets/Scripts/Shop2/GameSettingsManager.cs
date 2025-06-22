using UnityEngine;

// 게임의 전역 설정(오디오, 언어 등)을 관리하는 매니저 클래스
public class GameSettingsManager : MonoBehaviour
{
    public static GameSettingsManager Instance { get; private set; }    // 싱글턴 인스턴스

    [Header("Audio Settings")]
    [Range(0, 100)] public int masterVolume = 100;  // 마스터 볼륨 (0~100)
    [Range(0, 100)] public int bgmVolume = 100; // 배경음 볼륨
    [Range(0, 100)] public int sfxVolume = 100; // 효과음 볼륨
    public bool isMuted = false;    // 전체 음소거 여부

    [Header("Language Settings")]
    public SystemLanguage currentLanguage = SystemLanguage.Korean;  // 현재 언어 설정

    [Header("Audio Sources")]
    public AudioSource bgmSource;   // 배경음에 사용할 AudioSource
    public AudioSource[] sfxSources;    // 효과음에 사용할 AudioSource 배열


    private void Awake()
    {
        // 중복 인스턴스 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);  // 씬 전환 시 파괴되지 않도록 설정

        LoadSettings(); // 저장된 설정 불러오기
        ApplyAudioSettings();   // 오디오 볼륨 등 적용
    }

    // 현재 설정된 오디오 값을 AudioSource에 반영
    public void ApplyAudioSettings()
    {
        float masterFactor = isMuted ? 0f : masterVolume / 100f;    // 음소거면 0

        if (bgmSource)
            bgmSource.volume = masterFactor * (bgmVolume / 100f);   // 배경음 볼륨 계산

        foreach (var sfx in sfxSources)
        {
            if (sfx != null)
                sfx.volume = masterFactor * (sfxVolume / 100f); // 효과음 볼륨 계산
        }
    }

    // 현재 설정을 PlayerPrefs에 저장
    public void SaveSettings()
    {
        PlayerPrefs.SetInt("MasterVolume", masterVolume);
        PlayerPrefs.SetInt("BGMVolume", bgmVolume);
        PlayerPrefs.SetInt("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.SetString("Language", currentLanguage.ToString());
        PlayerPrefs.Save(); // 저장 실행
    }

    // PlayerPrefs에서 설정값 불러오기
    public void LoadSettings()
    {
        masterVolume = PlayerPrefs.GetInt("MasterVolume", 100);
        bgmVolume = PlayerPrefs.GetInt("BGMVolume", 100);
        sfxVolume = PlayerPrefs.GetInt("SFXVolume", 100);
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;

        string lang = PlayerPrefs.GetString("Language", SystemLanguage.Korean.ToString());
        System.Enum.TryParse(lang, out currentLanguage);

        // 파싱 실패 시 기본값으로 설정
        if (!System.Enum.TryParse(lang, out currentLanguage))
        {
            currentLanguage = SystemLanguage.Korean;
            Debug.LogWarning($"언어 파싱 실패: {lang}, 기본값(Korean)으로 설정됨"); // 디버그용 로그
        }
    }

    // 언어 설정을 바꾸고 저장
    public void SetLanguage(SystemLanguage lang)
    {
        currentLanguage = lang;
        SaveSettings(); // 저장
        // LocalizationManager 등에서 이후 반영
    }

    // 외부에서 배경음용 AudioSource 등록
    public void SetBGMSource(AudioSource newSource)
    {
        if (bgmSource == newSource)
            return;

        bgmSource = newSource;

        ApplyAudioSettings(); // 새 소스에 볼륨 적용
    }


}
