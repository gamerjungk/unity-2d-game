using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public enum Language { Korean, English }

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    public Language currentLanguage = Language.Korean;

    private Dictionary<string, string> koreanTexts = new Dictionary<string, string>();
    private Dictionary<string, string> englishTexts = new Dictionary<string, string>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadTexts(); // 언어 텍스트 로딩만 수행
    }

    private void Start()
    {
        // GameSettingsManager에서 언어 설정 받아오기
        if (GameSettingsManager.Instance != null)
        {
            currentLanguage = ConvertSystemLanguage(GameSettingsManager.Instance.currentLanguage);
        }
        else
        {
            Debug.LogWarning("GameSettingsManager.Instance is null! 언어 기본값으로 설정됨.");
        }

        // UI 텍스트 갱신
        LocalizationTarget[] targets = FindObjectsOfType<LocalizationTarget>();
        foreach (var t in targets)
            t.RefreshText();
    }

    private Language ConvertSystemLanguage(SystemLanguage sysLang)
    {
        return sysLang switch
        {
            SystemLanguage.English => Language.English,
            _ => Language.Korean
        };
    }

    private void LoadTexts()
    {
        koreanTexts["Settings"] = "설정";
        koreanTexts["Buy"] = "구매";
        koreanTexts["Equip"] = "장착";
        koreanTexts["Equipped"] = "장착중";
        koreanTexts["Speed"] = "속도";
        koreanTexts["Efficiency"] = "연비";
        koreanTexts["Capacity"] = "적재량";
        koreanTexts["Payment"] = "징수";
        koreanTexts["MasterVolume"] = "마스터 볼륨";
        koreanTexts["BGM"] = "배경음악";
        koreanTexts["SFX"] = "효과음";
        koreanTexts["Mute"] = "음소거";
        koreanTexts["Language"] = "언어";


        englishTexts["Settings"] = "Settings";
        englishTexts["Buy"] = "Buy";
        englishTexts["Equip"] = "Equip";
        englishTexts["Equipped"] = "Equipped";
        englishTexts["Speed"] = "Speed";
        englishTexts["Efficiency"] = "Oil Efficiency";
        englishTexts["Capacity"] = "Capacity";
        englishTexts["Payment"] = "Payment";
        englishTexts["MasterVolume"] = "Master Volume";
        englishTexts["BGM"] = "BGM";
        englishTexts["SFX"] = "Sound Effects";
        englishTexts["Mute"] = "Mute";
        englishTexts["Language"] = "Language";
    }

    public string GetText(string key)
    {
        string val;
        if (currentLanguage == Language.Korean)
        {
            return koreanTexts.TryGetValue(key, out val) ? val : key;
        }
        else
        {
            return englishTexts.TryGetValue(key, out val) ? val : key;
        }
    }
    public List<string> GetAllKeys()
    {
        var all = new HashSet<string>(koreanTexts.Keys);
        foreach (var k in englishTexts.Keys)
            all.Add(k);
        return new List<string>(all);
    }

    public void ChangeLanguage(Language lang)
    {
        currentLanguage = lang;
        LocalizationTarget[] targets = FindObjectsOfType<LocalizationTarget>();
        foreach (var t in targets)
        {
            t.RefreshText();
        }
    }
}

// 키 목록
public static class LocalizationKeys
{
    public static readonly string[] Keys = new string[]
    {
        "Settings", "Buy", "Equip", "Equipped", "Speed", "Efficiency", "Capacity", "Payment", "MasterVolume", "BGM", "SFX", "Mute", "Language"
        // 필요한 키 계속 추가
    };
}

