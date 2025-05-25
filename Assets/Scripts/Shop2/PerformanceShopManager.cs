using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public enum ShopTab
{
    Vehicle,
    Consumable,
    OneTime
}

public class PerformanceShopManager : MonoBehaviour
{
    public static PerformanceShopManager Instance { get; private set; }

    [Header("UI References")]
    public TextMeshProUGUI moneyText;
    public Transform shopPanel;
    public GameObject performanceItemSlotPrefab;
    [SerializeField] private Button payButton; // 💸 징세 내변 버튼
    [SerializeField] private GameObject vehicleSlotPrefab;
    [SerializeField] private GameObject consumableSlotPrefab;
    [SerializeField] private GameObject oneTimeSlotPrefab;

    [Header("Items")]
    public PerformanceItemSO[] allItems;

    private ShopTab currentTab = ShopTab.Vehicle;
    public bool showOneTimeItems = false; // 해당 턴에만 OneTime 탭 표시

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private IEnumerator Start()
    {
        yield return null; // GameDataManager 처리 기다리기

        GenerateShopSlots();
        UpdateMoneyUI();
        payButton.onClick.AddListener(TryPayNextStage);
    }

    public void OnTabSelected(int tabIndex)
    {
        currentTab = (ShopTab)tabIndex;
        GenerateShopSlots();
    }

    private void GenerateShopSlots()
    {
        foreach (Transform child in shopPanel)
            Destroy(child.gameObject);

        foreach (var item in allItems)
        {
            if (!IsItemInCurrentTab(item)) continue;

            var slotPrefab = GetPrefabForItem(item.itemType);
            var slot = Instantiate(slotPrefab, shopPanel);
            var slotComponent = slot.GetComponent<PerformanceItemSlot>();
            slotComponent.Setup(item);
            if (item.itemType == ItemType.Consumable)
            {
                slotComponent.EnableUseButton(() =>
                {
                    var ownedItem = GameDataManager.Instance.data.ownedItems.Find(x => x.itemId == item.name);
                    if (ownedItem != null && ownedItem.count > 0)
                    {
                        ownedItem.count--;
                        Debug.Log($"✨ {item.name} 사용함. 남은 수량: {ownedItem.count}");

                        if (ownedItem.count <= 0)
                            GameDataManager.Instance.data.ownedItems.Remove(ownedItem);

                        GameDataManager.Instance.Save();
                        RefreshAllSlots();
                    }
                });
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(shopPanel.GetComponent<RectTransform>());
    }


    private bool IsItemInCurrentTab(PerformanceItemSO item)
    {
        return currentTab switch
        {
            ShopTab.Vehicle => item.itemType == ItemType.Permanent || item.itemType == ItemType.Vehicle,
            ShopTab.Consumable => item.itemType == ItemType.Consumable,
            ShopTab.OneTime => showOneTimeItems && item.itemType == ItemType.OneTime,
            _ => false,
        };
    }

    public void BuySelectedItem(PerformanceItemSO item)
    {
        if (PerformanceInventoryManager.Instance.IsOwned(item))
        {
            Debug.Log("이미 소유한 아이템입니다.");
            return;
        }

        if (GameDataManager.Instance.data.money < item.price)
        {
            Debug.Log("골드 부족!");
            return;
        }

        GameDataManager.Instance.data.money -= item.price;
        PerformanceInventoryManager.Instance.BuyItem(item);
        RefreshAllSlots();

        UpdateMoneyUI();
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

    private void UpdateMoneyUI()
    {
        if (GameDataManager.Instance == null || GameDataManager.Instance.data == null)
        {
            Debug.LogWarning("❗ GameDataManager 또는 data가 아직 초기화되지 않았습니다.");
            return;
        }

        moneyText.text = GameDataManager.Instance.data.money.ToString() + "원";
        Debug.Log("💰 돈 UI 갱신됨: " + moneyText.text);
    }

    // 💸 집세 납부 로직
    public void TryPayNextStage()
    {
        bool success = GameDataManager.Instance.TryPay();

        if (success)
        {
            Debug.Log("✅ 납부 성공! 현재 납부 단계: " + GameDataManager.Instance.data.paidStageIndex);
            UpdateMoneyUI();
        }
        else
        {
            Debug.Log("❌ 돈이 부족하여 납부할 수 없습니다.");
        }
    }

    //프리팹 선택 분기
// 🔧 이렇게 바꿔주세요
    private GameObject GetPrefabForItem(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Consumable => consumableSlotPrefab,
            ItemType.OneTime => oneTimeSlotPrefab,
            _ => vehicleSlotPrefab,
        };
    }


}
