using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    public static GameSettingsManager Instance { get; private set; }

    [Header("Audio Settings")]
    [Range(0, 100)] public int masterVolume = 100;
    [Range(0, 100)] public int bgmVolume = 100;
    [Range(0, 100)] public int sfxVolume = 100;
    public bool isMuted = false;

    [Header("Language Settings")]
    public SystemLanguage currentLanguage = SystemLanguage.Korean;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource[] sfxSources;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
        ApplyAudioSettings();
    }

    public void ApplyAudioSettings()
    {
        float masterFactor = isMuted ? 0f : masterVolume / 100f;

        if (bgmSource)
            bgmSource.volume = masterFactor * (bgmVolume / 100f);

        foreach (var sfx in sfxSources)
        {
            if (sfx != null)
                sfx.volume = masterFactor * (sfxVolume / 100f);
        }
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("MasterVolume", masterVolume);
        PlayerPrefs.SetInt("BGMVolume", bgmVolume);
        PlayerPrefs.SetInt("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.SetString("Language", currentLanguage.ToString());
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        masterVolume = PlayerPrefs.GetInt("MasterVolume", 100);
        bgmVolume = PlayerPrefs.GetInt("BGMVolume", 100);
        sfxVolume = PlayerPrefs.GetInt("SFXVolume", 100);
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;

        string lang = PlayerPrefs.GetString("Language", SystemLanguage.Korean.ToString());
        System.Enum.TryParse(lang, out currentLanguage);

        // 디버그용
        if (!System.Enum.TryParse(lang, out currentLanguage))
        {
            Debug.LogWarning($"언어 파싱 실패: {lang}, 기본값(Korean)으로 설정됨");
        }
    }

    public void SetLanguage(SystemLanguage lang)
    {
        currentLanguage = lang;
        SaveSettings();
        // 이후 LocalizationManager 같은 데서 반영
    }

    public void SetBGMSource(AudioSource newSource)
    {
        if (bgmSource == newSource)
            return;

        bgmSource = newSource;

        ApplyAudioSettings(); // 볼륨 반영
        if (!bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

}
