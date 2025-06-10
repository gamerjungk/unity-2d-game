using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    public static event Action OnDataLoaded; // 이벤트 추가
    public static event Action OnDataReloaded;
    public bool IsInitialized { get; private set; } = false;

    public GameData data;

    private void Awake()
    {
        // 중복 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환에도 유지

        Load(); // 항상 먼저 실행
    }


    private void Start()
    {
        StartCoroutine(DelayedInit());
    }

    private IEnumerator DelayedInit()
    {
        yield return null;

        // ❗다른 시스템보다 늦게 실행되도록 1프레임 대기
        // PerformanceInventoryManager.Instance?.LoadFromGameData(data);
        IsInitialized = true;

        Debug.Log("✅ GameDataManager 초기화 완료");
        OnDataLoaded?.Invoke();
    }

    public void Save()
    {
        SaveManager.Save(data);
        Debug.Log("📁 저장경로: " + Application.persistentDataPath);
    }

    public void Load()
    {
        data = SaveManager.Load();

        if (IsFirstPlay() || data.ownedItems == null || data.ownedItems.Count == 0)
        {
            Debug.Log("🎉 최초 실행 - 기본값 세팅");
            data.gold = 100;
            data.money = 10000;
            data.turn = 5;
            data.paidStageIndex = 0;
            data.currentRound = 1;
            data.ownedItems = new List<SerializableItem>();

            Save();
            PlayerPrefs.SetInt("HasPlayed", 1);

            OnDataReloaded?.Invoke(); // ✅ 꼭 있어야 함
        }

        PerformanceInventoryManager.Instance?.LoadFromGameData(data);
    }


    private bool IsFirstPlay() => PlayerPrefs.GetInt("HasPlayed", 0) == 0;
    private void OnApplicationPause(bool pause) { if (pause) Save(); }
    private void OnApplicationQuit() { Save(); }

    public int GetRequiredPayment()
    {
        return 1000 + (data.paidStageIndex * 1000);
    }

    public bool TryPay()
    {
        int currentIndex = data.paidStageIndex;
        int required = 1000 + (currentIndex * 1000); // 납부액 확정 (UI와 동일 기준)

        Debug.Log($"납부 시도 - 현재 paidStageIndex: {currentIndex}, 납부액: {required}, 현재 보유금액: {data.money}");

        if (data.money >= required)
        {
            data.money -= required;
            data.paidStageIndex++;

            Debug.Log($"납부 성공. 남은 금액: {data.money}, 다음 paidStageIndex: {data.paidStageIndex}");

            SaveManager.Save(data);
            return true;
        }
        Debug.Log("돈 부족으로 납부 실패");
        return false;
    }


    public void ClearOneTimeItems()
    {
        int before = data.ownedItems.Count;
        data.ownedItems.RemoveAll(item => item.itemType == ItemType.OneTime);
        int after = data.ownedItems.Count;
        Debug.Log($"🧹 라운드 종료로 일회성 아이템 {before - after}개 제거됨");
    }

    public void ResetGameData()
    {
        string path = Application.persistentDataPath + "/save.json";
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("🗑️ save.json 삭제됨");
        }

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("🧼 PlayerPrefs 초기화됨");

        PerformanceInventoryManager.Instance.ClearAll();
        
        Load(); // ✅ 다시 불러오기
        OnDataReloaded?.Invoke(); // ✅ 다른 시스템에게 알려줌
    }
    

    public void AddMoney(int amount)
    {
        data.money += amount;
        Debug.Log("현재 돈: " + data.money);
    }
    
}
