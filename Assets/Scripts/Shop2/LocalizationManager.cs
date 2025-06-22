using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public enum Language { Korean, English }    // 지원하는 언어 열거형 정의

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }    // 싱글턴 인스턴스

    public Language currentLanguage = Language.Korean;  // 현재 사용 중인 언어 (기본값: 한국어)

    private Dictionary<string, string> koreanTexts = new Dictionary<string, string>();  // 한글 텍스트 딕셔너리
    private Dictionary<string, string> englishTexts = new Dictionary<string, string>(); // 영어 텍스트 딕셔너리

    private void Awake()
    {   
        // 싱글턴 중복 방지 및 초기화
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);  // 씬 전환 시에도 파괴되지 않도록 유지

        LoadTexts(); // 언어별 텍스트 로드
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

        // 현재 씬의 모든 LocalizationTarget 텍스트들을 찾아서 새 언어로 갱신
        var targets = FindObjectsByType<LocalizationTarget>(FindObjectsSortMode.None); // or .InstanceID if 필요

        foreach (var t in targets)
            t.RefreshText();    // 텍스트 키 기반으로 새 문자열 적용
    }

    // Unity의 SystemLanguage를 내부 Language enum으로 변환
    private Language ConvertSystemLanguage(SystemLanguage sysLang)
    {
        return sysLang switch
        {
            SystemLanguage.English => Language.English,
            _ => Language.Korean    // 나머지는 전부 한국어 처리
        };
    }

    // 언어별 텍스트를 Dictionary에 미리 등록
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
        koreanTexts["MasterVolume"] = "마스터볼륨";
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
        englishTexts["MasterVolume"] = "MasterVolume";
        englishTexts["BGM"] = "BGM";
        englishTexts["SFX"] = "Sound Effects";
        englishTexts["Mute"] = "Mute";
        englishTexts["Language"] = "Language";
    }

    // 키에 해당하는 텍스트 반환 (없으면 key 자체 반환)
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

    // 등록된 모든 키 목록 반환 (한국어+영어 기준으로 통합 중복 제거)
    public List<string> GetAllKeys()
    {
        var all = new HashSet<string>(koreanTexts.Keys);    // 중복 방지를 위한 HashSet
        foreach (var k in englishTexts.Keys)    
            all.Add(k); // 영어 키 추가 (중복 제거됨)
        return new List<string>(all);   // 리스트로 변환하여 반환
    }

    // 언어 변경 시 호출되는 함수 (모든 텍스트 갱신)
    public void ChangeLanguage(Language lang)
    {
        currentLanguage = lang; // 현재 언어 변경

        // 현재 씬 내 모든 LocalizationTarget에 대해 텍스트 갱신
        LocalizationTarget[] targets = FindObjectsByType<LocalizationTarget>(FindObjectsSortMode.None);
        foreach (var t in targets)
        {
            t.RefreshText();    // 새 언어로 UI 텍스트 갱신
        }
    }
}

// 키 목록
public static class LocalizationKeys
{   
    // 사용할 수 있는 모든 문자열 키들을 미리 정리해둔 배열
    public static readonly string[] Keys = new string[]
    {
        "Settings", "Buy", "Equip", "Equipped", "Speed", "Efficiency", "Capacity", "Payment", "MasterVolume", "BGM", "SFX", "Mute", "Language"
        // 필요한 키 계속 추가
    };
}

