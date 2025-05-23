using UnityEngine;

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

    public void Save()
    {
        SaveManager.Save(data);
    }

    public void Load()
    {
        data = SaveManager.Load();
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
}
