using System.Collections.Generic;
using UnityEngine;
using System;

public class PerformanceInventoryManager : MonoBehaviour
{
    public static PerformanceInventoryManager Instance { get; private set; }
    public static event Action OnInventoryLoaded;
    public HashSet<PerformanceItemSO> ownedItems = new();
    public HashSet<string> ownedItemIds = new();
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

    public bool IsOwned(PerformanceItemSO item)
    {
        bool result = ownedItemIds.Contains(item.itemId);
        Debug.Log($"[IsOwned] {item.itemId} → {result}");
        return result;
    }

    public void BuyItem(PerformanceItemSO item)
    {
        if (!ownedItems.Contains(item))
            ownedItems.Add(item);

        if (!ownedItemIds.Contains(item.itemId))
            ownedItemIds.Add(item.itemId);

        var data = GameDataManager.Instance.data;
        var existing = data.ownedItems.Find(x => x.itemId == item.itemId);

        if (existing != null)
            existing.count++;
        else
            data.ownedItems.Add(new SerializableItem
            {
                itemId = item.itemId,
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

        // ✅ 장착 정보 GameData에도 반영
        foreach (var owned in GameDataManager.Instance.data.ownedItems)
        {
            if (owned.itemId == item.itemId) // 🔄 인스턴스 비교 → 문자열 비교로 변경
            {
                owned.isEquipped = true;
            }
            else if (!category.allowMultipleEquip)
            {
                var tempItemSO = Resources.Load<PerformanceItemSO>($"Items/Shop2/item/{owned.itemId}");
                if (tempItemSO != null && tempItemSO.category == category)
                    owned.isEquipped = false;
            }
        }

        GameDataManager.Instance.Save(); // 변경 사항 저장
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
        ownedItemIds.Clear();

        foreach (var item in data.ownedItems)
        {
            Debug.Log($"[SAVE DATA] {item.itemId}, count: {item.count}, equipped: {item.isEquipped}");
            var itemSO = Resources.Load<PerformanceItemSO>($"Items/Shop2/item/{item.itemId}");
            if (itemSO != null)
            {
                ownedItems.Add(itemSO);
                ownedItemIds.Add(item.itemId);

                if (item.isEquipped)
                {
                    // 🚫 EquipItem() 호출하지 말고, 직접 세팅만 해
                    if (itemSO.category.allowMultipleEquip)
                    {
                        if (!equippedItemsMulti.ContainsKey(itemSO.category))
                            equippedItemsMulti[itemSO.category] = new List<PerformanceItemSO>();

                        if (!equippedItemsMulti[itemSO.category].Contains(itemSO))
                            equippedItemsMulti[itemSO.category].Add(itemSO);
                    }
                    else
                    {
                        equippedItemsByCategory[itemSO.category] = itemSO;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"❗ 아이템 SO 로드 실패: {item.itemId}");
            }
        }
        OnInventoryLoaded?.Invoke();
    }

    public void ClearAll()
    {
        ownedItems.Clear();
        equippedItemsByCategory.Clear();
        equippedItemsMulti.Clear();
        Debug.Log("🔁 PerformanceInventoryManager 데이터 초기화됨");
    }

    private void OnEnable()
    {
        GameDataManager.OnDataLoaded += HandleGameDataLoaded;

        // 최초 수동 호출 (놓친 경우)
        if (GameDataManager.Instance != null && GameDataManager.Instance.IsInitialized)
            HandleGameDataLoaded();
    }
    private void OnDisable()
    {
        GameDataManager.OnDataLoaded -= HandleGameDataLoaded;
    }

    private void HandleGameDataLoaded()
    {
        LoadFromGameData(GameDataManager.Instance.data);
    }


}