using UnityEngine;
using System.IO;   
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    public GameData data;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 한 프레임 뒤에 초기화
        StartCoroutine(DelayedInit());
    }

    private IEnumerator DelayedInit()
    {
        yield return null; // 한 프레임 대기
        PerformanceInventoryManager.Instance?.LoadFromGameData(data);
    }

    public void Save()
    {
        SaveManager.Save(data);
        //저장경로 로그로 보여주기
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
            /*
                        data.ownedItems = new List<SerializableItem>()
                        {
                            new SerializableItem
                            {
                                itemId = "StarterPack",
                                count = 1,
                                itemType = ItemType.Permanent,
                                isEquipped = false,
                                isUnlocked = true
                            }
                        };
            */
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
        // 예: 1단계당 1000원 증가
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
    
    // 게임 초기화
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
