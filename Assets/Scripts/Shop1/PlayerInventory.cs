// 수정 대상: PlayerInventory.cs
// 기존 딕셔너리 구조 유지하면서 GameDataManager 구조에 맞춰 저장/불러오기 통합

using System.Collections.Generic;
using UnityEngine;

public static class PlayerInventory
{
    public static Dictionary<string, int> ownedItemCounts = new(); // itemId 기반으로 변경

    public static bool HasItem(ItemSO item) => ownedItemCounts.ContainsKey(item.name) && ownedItemCounts[item.name] > 0;

    public static int GetCount(ItemSO item) => ownedItemCounts.TryGetValue(item.name, out int count) ? count : 0;

    public static void AddItem(ItemSO item, int count = 1)
    {
        if (!ownedItemCounts.ContainsKey(item.name))
            ownedItemCounts[item.name] = 0;
        ownedItemCounts[item.name] += count;
        SyncToGameData(item);
        GameDataManager.Instance.Save();
    }

    public static bool UseItem(ItemSO item, int count = 1)
    {
        if (!HasItem(item) || ownedItemCounts[item.name] < count)
            return false;

        ownedItemCounts[item.name] -= count;
        SyncToGameData(item);
        GameDataManager.Instance.Save();
        return true;
    }

    public static void LoadFromGameData(GameData data)
    {
        ownedItemCounts.Clear();
        foreach (var entry in data.ownedItems)
        {
            if (ownedItemCounts.ContainsKey(entry.itemId))
                ownedItemCounts[entry.itemId] += entry.count;
            else
                ownedItemCounts[entry.itemId] = entry.count;
        }
    }

    private static void SyncToGameData(ItemSO item)
    {
        var gameData = GameDataManager.Instance.data;
        var existing = gameData.ownedItems.Find(x => x.itemId == item.name);

        if (ownedItemCounts[item.name] <= 0)
        {
            if (existing != null)
                gameData.ownedItems.Remove(existing);
            return;
        }

        if (existing != null)
        {
            existing.count = ownedItemCounts[item.name];
        }
        else
        {
            gameData.ownedItems.Add(new SerializableItem
            {
                itemId = item.name,
                count = ownedItemCounts[item.name],
                itemType = ItemType.Consumable,
                isEquipped = false,
                isUnlocked = true
            });
        }
    }

    public static void Clear()
    {
        ownedItemCounts.Clear();
    }
}
