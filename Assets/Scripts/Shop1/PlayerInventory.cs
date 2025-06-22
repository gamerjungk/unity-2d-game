// 수정 대상: PlayerInventory.cs
// 기존 딕셔너리 구조 유지하면서 GameDataManager 구조에 맞춰 저장/불러오기 통합

using System.Collections.Generic;
using UnityEngine;

// 플레어가 보유한 아이템을 관리하는 정적 인벤토리 클래스
// 기존 딕셔너리 구조를 유지하며 GameDataManager와 연동하여 저장 및 불러오기 기능 포함
public static class PlayerInventory
{   
    // itemId(string) 기준으로 보유 수량을 저장하는 딕셔너리
    public static Dictionary<string, int> ownedItemCounts = new();

    // 아이템 보유 여부 확인 (보유 수량이 1 이상인지 확인)
    public static bool HasItem(ItemSO item) => ownedItemCounts.ContainsKey(item.name) && ownedItemCounts[item.name] > 0;

    // 특정 아이템의 보유 수량 반환 (없으면 0)
    public static int GetCount(ItemSO item) => ownedItemCounts.TryGetValue(item.name, out int count) ? count : 0;

    // 아이템 추가 (기본 수량: 1)
    public static void AddItem(ItemSO item, int count = 1)
    {
        if (!ownedItemCounts.ContainsKey(item.name))
            ownedItemCounts[item.name] = 0;

        ownedItemCounts[item.name] += count;

        SyncToGameData(item);   // GameData에도 반영
        GameDataManager.Instance.Save();    // 저장
    }

    // 아이템 사용 시 수량 차감 (성공 시 true 반환)
    public static bool UseItem(ItemSO item, int count = 1)
    {
        if (!HasItem(item) || ownedItemCounts[item.name] < count)
            return false;

        ownedItemCounts[item.name] -= count;
        SyncToGameData(item);   // GameData에도 반영
        GameDataManager.Instance.Save();    // 저장
        return true;
    }

    // GameData에서 소유 아이템 정보를 불러와 딕셔너리에 반영
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

    // 현재 딕셔너리 상태를 GameDataManager에 동기화
    private static void SyncToGameData(ItemSO item)
    {
        var gameData = GameDataManager.Instance.data;
        var existing = gameData.ownedItems.Find(x => x.itemId == item.name);

        // 수량이 0 이하인 경우, GameData에서 해당 항목 제거
        if (ownedItemCounts[item.name] <= 0)
        {
            if (existing != null)
                gameData.ownedItems.Remove(existing);
            return;
        }

        // 이미 GameData에 존재하면 수량만 갱신
        if (existing != null)
        {
            existing.count = ownedItemCounts[item.name];
        }
        else    // 존재하지 않으면 새로 추가
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

    // 인벤토리 초기화 (모든 소유 아이템 제거)
    public static void Clear()
    {
        ownedItemCounts.Clear();
    }
}
