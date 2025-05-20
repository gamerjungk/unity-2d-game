using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class SerializableItem
{
    public string itemName;
    public int count;
}

[System.Serializable]
public class GameData
{
    public int gold;
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
