using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    public static event Action OnDataLoaded; // ✅ 이벤트 추가

    public GameData data;

    void Awake()
    {
        Instance = this;
        Load(); // 항상 실행
    }


    private void Start()
    {
        StartCoroutine(DelayedInit());
    }

    private IEnumerator DelayedInit()
    {
        yield return null;
        PerformanceInventoryManager.Instance?.LoadFromGameData(data);

        OnDataLoaded?.Invoke(); // ✅ 데이터 초기화 완료 알림
    }

    public void Save()
    {
        SaveManager.Save(data);
        Debug.Log("📁 저장경로: " + Application.persistentDataPath);
    }

    public void Load()
    {
        data = SaveManager.Load();

        if (IsFirstPlay())
        {
            Debug.Log("🎉 최초 실행 - 기본값 세팅");
            data.gold = 100;
            data.money = 10000;
            data.turn = 0;
            data.paidStageIndex = 0;
            data.currentRound = 1;
            Save();
            PlayerPrefs.SetInt("HasPlayed", 1);
        }

        PerformanceInventoryManager.Instance?.LoadFromGameData(data);
    }

    private bool IsFirstPlay()
    {
        return PlayerPrefs.GetInt("HasPlayed", 0) == 0;
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
            Save();
    }

    void OnApplicationQuit()
    {
        Save();
    }

    public int GetRequiredPayment()
    {
        return 1000 + (data.paidStageIndex * 1000);
    }

    public bool TryPay()
    {
        int required = GetRequiredPayment();
        if (data.money >= required)
        {
            data.money -= required;
            data.paidStageIndex++;
            SaveManager.Save(data);
            return true;
        }
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

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
