using UnityEngine;
using System.Collections.Generic;
using System.IO;

public enum ItemType
{
    Consumable,     // 소모형 (예: 연료, 치료제)
    OneTime,        // 일회성 잠금 해제 (예: 특정 퀘스트)
    Permanent,      // 영구형 (예: 차량, 스킨)
    Vehicle,        // 탈것
    Skin,           // 외형
    Currency        // 재화 (예: 골드)
}

[System.Serializable]
public class SerializableItem
{
    public string itemId;           // SO 파일 이름 또는 고유 ID
    public int count;               // 보유 수량
    public ItemType itemType;       // 아이템 유형
    public bool isEquipped;         // 장착 여부
    public float duration;          // 시간제 아이템일 경우 (초 단위)
    public bool isUnlocked;         // 잠금 해제 여부 (OneTime용)
}
[System.Serializable]
public class GameData
{   
    //재화 및 턴
    public int gold;
    public int money;
    public int turn;

    // 해당 납부 단계에서 최대 플레이 가능한 라운드 수
    public int maxTurnsPerRound = 5;

    //납부 관련
    public int currentRound;
    public int paidStageIndex;

    public List<SerializableItem> ownedItems = new List<SerializableItem>();
}

public static class SaveManager
{
    private static string SavePath => Application.persistentDataPath + "/save.json";

    public static void Save(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static GameData Load()
    {
        if (!File.Exists(SavePath))
            return new GameData();

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<GameData>(json);
    }
}
