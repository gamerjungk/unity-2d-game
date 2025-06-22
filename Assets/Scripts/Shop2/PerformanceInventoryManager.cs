using System.Collections.Generic;
using UnityEngine;
using System;

public class PerformanceInventoryManager : MonoBehaviour
{
    public static PerformanceInventoryManager Instance { get; private set; }    // 싱글턴 인스턴스
    public static event Action OnInventoryLoaded;   // 인벤토리 로드 완료 시 발생하는 이벤트
    public HashSet<PerformanceItemSO> ownedItems = new();   // 보유 아이템 SO 목록 (중복 방지)
    public HashSet<string> ownedItemIds = new();    // 보유 아이템 ID 목록 (빠른 비교용)
    public Dictionary<PerformanceCategorySO, PerformanceItemSO> equippedItemsByCategory = new();    // 카테고리당 단일 장착 아이템
    public Dictionary<PerformanceCategorySO, List<PerformanceItemSO>> equippedItemsMulti = new();   // 카테고리당 다중 장착 아이템 목록

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);    // 중복 인스턴스 제거
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);  // 씬 전환 시에도 유지
    }

    public bool IsOwned(PerformanceItemSO item)
    {
        bool result = ownedItemIds.Contains(item.itemId);   // itemId 기준 보유 여부 확인
        Debug.Log($"[IsOwned] {item.itemId} → {result}");
        return result;
    }

    public void BuyItem(PerformanceItemSO item)
    {
        if (!ownedItems.Contains(item))
            ownedItems.Add(item);   // 중복 없이 아이템 SO 추가

        if (!ownedItemIds.Contains(item.itemId))
            ownedItemIds.Add(item.itemId);  // 중복 없이 itemId 추가

        var data = GameDataManager.Instance.data;
        var existing = data.ownedItems.Find(x => x.itemId == item.itemId);  // 저장 데이터에서 해당 아이템 검색


        if (existing != null)
            existing.count++;   // 이미 있다면 수량 증가
        else
            data.ownedItems.Add(new SerializableItem    // 없으면 새로 추가
            {
                itemId = item.itemId,
                count = 1,
                itemType = item.itemType, // 필요 시 조건 분기
                isEquipped = false,
                isUnlocked = true
            });

        GameDataManager.Instance.Save();    // 저장
    }



    public void EquipItem(PerformanceCategorySO category, PerformanceItemSO item)
    {
        if (category.allowMultipleEquip)
        {
            if (!equippedItemsMulti.ContainsKey(category))
                equippedItemsMulti[category] = new();   // 카테고리 초기화

            if (!equippedItemsMulti[category].Contains(item))
                equippedItemsMulti[category].Add(item); // 중복 없이 추가
        }
        else
        {
            equippedItemsByCategory[category] = item;   // 단일 장착 처리
        }

        // // GameData 내 장착 정보 갱신
        foreach (var owned in GameDataManager.Instance.data.ownedItems)
        {
            if (owned.itemId == item.itemId) // 인스턴스 비교 → 문자열 비교로 변경
            {
                owned.isEquipped = true;    // 장착 상태 설정
            }
            else if (!category.allowMultipleEquip)
            {
                var tempItemSO = Resources.Load<PerformanceItemSO>($"Items/Shop2/item/{owned.itemId}"); // 다른 아이템 로드
                if (tempItemSO != null && tempItemSO.category == category)
                    owned.isEquipped = false;   // 같은 카테고리라면 장착 해제
            }
        }

        GameDataManager.Instance.Save(); // 변경 사항 저장
    }



    public bool IsEquipped(PerformanceCategorySO category, PerformanceItemSO item)
    {
        if (category.allowMultipleEquip)
        {
            return equippedItemsMulti.ContainsKey(category) && equippedItemsMulti[category].Contains(item); // 다중 장착 확인
        }
        else
        {
            return equippedItemsByCategory.TryGetValue(category, out var equipped) && equipped == item; // 단일 장착 확인
        }
    }

    public void LoadFromGameData(GameData data)
    {
        ownedItems.Clear();
        ownedItemIds.Clear();

        foreach (var item in data.ownedItems)
        {
            Debug.Log($"[SAVE DATA] {item.itemId}, count: {item.count}, equipped: {item.isEquipped}");
            var itemSO = Resources.Load<PerformanceItemSO>($"Items/Shop2/item/{item.itemId}");  // SO 로드
            if (itemSO != null)
            {
                ownedItems.Add(itemSO);
                ownedItemIds.Add(item.itemId);

                if (item.isEquipped)
                {
                    // // EquipItem을 호출하지 않고 수동으로 장착 정보 반영
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
                Debug.LogWarning($"아이템 SO 로드 실패: {item.itemId}");    // 리소스 로드 실패 경고(디버그용)
            }
        }
        OnInventoryLoaded?.Invoke();    // 인벤토리 로드 이벤트 호출
    }

    public void ClearAll()
    {
        ownedItems.Clear(); // 보유 아이템 초기화
        equippedItemsByCategory.Clear();    // 단일 장착 초기화
        equippedItemsMulti.Clear(); // 다중 장착 초기화
        Debug.Log("🔁 PerformanceInventoryManager 데이터 초기화됨");
    }

    private void OnEnable()
    {
        GameDataManager.OnDataLoaded += HandleGameDataLoaded;   // 데이터 로드 이벤트 구독

        // 인스턴스가 이미 초기화된 경우 수동 호출
        if (GameDataManager.Instance != null && GameDataManager.Instance.IsInitialized)
            HandleGameDataLoaded();
    }
    private void OnDisable()
    {
        GameDataManager.OnDataLoaded -= HandleGameDataLoaded;   // 이벤트 구독 해제
    }

    private void HandleGameDataLoaded()
    {
        LoadFromGameData(GameDataManager.Instance.data);    // 저장 데이터로 인벤토리 불러오기
    }


}