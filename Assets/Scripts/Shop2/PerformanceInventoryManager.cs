using System.Collections.Generic;
using UnityEngine;

public class PerformanceInventoryManager : MonoBehaviour
{
    public static PerformanceInventoryManager Instance { get; private set; }

    public HashSet<PerformanceItemSO> ownedItems = new();
    public Dictionary<PerformanceCategorySO, PerformanceItemSO> equippedItemsByCategory = new();
    public Dictionary<PerformanceCategorySO, List<PerformanceItemSO>> equippedItemsMulti = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool IsOwned(PerformanceItemSO item) => ownedItems.Contains(item);

    public void BuyItem(PerformanceItemSO item)
    {
        if (!ownedItems.Contains(item))
            ownedItems.Add(item);

        var data = GameDataManager.Instance.data;
        var existing = data.ownedItems.Find(x => x.itemId == item.name);

        if (existing != null)
            existing.count++;
        else
            data.ownedItems.Add(new SerializableItem
            {
                itemId = item.name,
                count = 1,
                itemType = item.itemType, // 필요 시 조건 분기
                isEquipped = false,
                isUnlocked = true
            });

        GameDataManager.Instance.Save();
    }



    public void EquipItem(PerformanceCategorySO category, PerformanceItemSO item)
    {
        if (category.allowMultipleEquip)
        {
            if (!equippedItemsMulti.ContainsKey(category))
                equippedItemsMulti[category] = new();

            if (!equippedItemsMulti[category].Contains(item))
                equippedItemsMulti[category].Add(item);
        }
        else
        {
            equippedItemsByCategory[category] = item;
        }
    }

    public bool IsEquipped(PerformanceCategorySO category, PerformanceItemSO item)
    {
        if (category.allowMultipleEquip)
        {
            return equippedItemsMulti.ContainsKey(category) && equippedItemsMulti[category].Contains(item);
        }
        else
        {
            return equippedItemsByCategory.TryGetValue(category, out var equipped) && equipped == item;
        }
    }

    public void LoadFromGameData(GameData data)
    {
        ownedItems.Clear();

        foreach (var item in data.ownedItems)
        {
            var itemSO = Resources.Load<PerformanceItemSO>($"Items/{item.itemId}");
            if (itemSO != null)
            {
                ownedItems.Add(itemSO);

                // 장착 정보 복원
                if (item.isEquipped)
                {
                    EquipItem(itemSO.category, itemSO);
                }
            }
            else
            {
                Debug.LogWarning($"❗ 아이템 SO 로드 실패: {item.itemId}");
            }
        }
    }


}