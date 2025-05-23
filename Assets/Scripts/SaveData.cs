using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class SerializableItem
{
    public string itemName;     // 아이템 이름 (예: "FuelPack", "BikeSkin")
    public int count;           // 보유 개수
    public bool isPermanent;    // true면 영구 아이템, false면 소모성
}

[System.Serializable]
public class GameData
{
    public int gold;
    public int money;
    public int turn;

    // ✅ 납부 시스템 관련
    public int currentRound;           // 현재 몇 라운드째인지
    public int paidStageIndex;         // 몇 단계 납부금까지 냈는지

    // ✅ 아이템
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
