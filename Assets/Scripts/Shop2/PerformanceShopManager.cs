using UnityEngine;
using UnityEngine.UI;

public class PerformanceShopManager : MonoBehaviour
{
    public static PerformanceShopManager Instance { get; private set; }

    [Header("UI References")]
    public Transform shopPanel;
    public GameObject performanceItemSlotPrefab;

    [Header("Items")]
    public PerformanceItemSO[] performanceItems;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        GenerateShopSlots();
    }

    private void GenerateShopSlots()
    {
        foreach (var item in performanceItems)
        {
            GameObject slot = Instantiate(performanceItemSlotPrefab, shopPanel, false);
            slot.GetComponent<PerformanceItemSlot>().Setup(item);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(shopPanel.GetComponent<RectTransform>());
    }

    public void BuySelectedItem(PerformanceItemSO item)
    {
        if (PerformanceInventoryManager.Instance.IsOwned(item))
        {
            Debug.Log("이미 소유한 아이템입니다.");
            return;
        }

        if (GameManager.gold < item.price)
        {
            Debug.Log("골드 부족!");
            return;
        }

        GameManager.gold -= item.price;
        PerformanceInventoryManager.Instance.BuyItem(item);
        RefreshAllSlots();
    }

    public void EquipSelectedItem(PerformanceItemSO item)
    {
        PerformanceInventoryManager.Instance.EquipItem(item.category, item);
        RefreshAllSlots();
    }

    private void RefreshAllSlots()
    {
        foreach (Transform child in shopPanel)
        {
            PerformanceItemSlot slot = child.GetComponent<PerformanceItemSlot>();
            if (slot != null)
                slot.Refresh();
        }
    }
}