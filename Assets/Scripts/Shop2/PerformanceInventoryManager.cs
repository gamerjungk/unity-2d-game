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
}