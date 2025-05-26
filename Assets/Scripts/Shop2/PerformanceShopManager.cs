using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PerformanceShopManager : MonoBehaviour
{
    public static PerformanceShopManager Instance { get; private set; }

    [Header("탭별 패널")]
    [SerializeField] private Transform vehiclePanel;
    [SerializeField] private Transform consumablePanel;
    [SerializeField] private Transform oneTimePanel;

    [Header("슬롯 프리팹")]
    [SerializeField] private GameObject vehicleSlotPrefab;
    [SerializeField] private GameObject consumableSlotPrefab;
    [SerializeField] private GameObject oneTimeSlotPrefab;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI turnStatusText;
    [SerializeField] private TextMeshProUGUI paymentAmountText;
    [SerializeField] private Button payButton;

    [Header("아이템 데이터")]
    public PerformanceItemSO[] allItems;

    private ShopTab currentTab = ShopTab.Vehicle;
    private const int totalTurnsPerRound = 5;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void OnEnable()
    {
        GameDataManager.OnDataLoaded += OnGameDataReady;
    }

    private void OnDisable()
    {
        GameDataManager.OnDataLoaded -= OnGameDataReady;
    }

    private void OnGameDataReady()
    {
        vehiclePanel.gameObject.SetActive(true);
        consumablePanel.gameObject.SetActive(false);
        oneTimePanel.gameObject.SetActive(false);

        UpdateMoneyUI();
        UpdateTurnAndPaymentUI();
        GenerateShopSlots();
        payButton.onClick.AddListener(TryPayNextStage);
    }

    public void OnTabSelected(int tabIndex)
    {
        currentTab = (ShopTab)tabIndex;

        // 탭 전환 시 패널 전환
        vehiclePanel.gameObject.SetActive(currentTab == ShopTab.Vehicle);
        consumablePanel.gameObject.SetActive(currentTab == ShopTab.Consumable);
        oneTimePanel.gameObject.SetActive(false);

        GenerateShopSlots();
    }

    private void GenerateShopSlots()
    {
        Transform targetPanel = GetCurrentPanel();

        foreach (Transform child in targetPanel)
            Destroy(child.gameObject);

        foreach (var item in allItems)
        {
            if (!IsItemInCurrentTab(item)) continue;

            var prefab = GetPrefabForItem(item.itemType);
            var slot = Instantiate(prefab, targetPanel);

            if (slot.TryGetComponent(out PerformanceItemSlot generalSlot))
            {
                generalSlot.Setup(item);

                if (item.itemType == ItemType.Consumable)
                {
                    generalSlot.EnableUseButton(() =>
                    {
                        var ownedItem = GameDataManager.Instance.data.ownedItems.Find(x => x.itemId == item.name);
                        if (ownedItem != null && ownedItem.count > 0)
                        {
                            ownedItem.count--;
                            if (ownedItem.count <= 0)
                                GameDataManager.Instance.data.ownedItems.Remove(ownedItem);

                            GameDataManager.Instance.Save();
                            PerformanceInventoryManager.Instance.LoadFromGameData(GameDataManager.Instance.data);
                            RefreshAllSlots();
                        }
                    });
                }
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(targetPanel.GetComponent<RectTransform>());
    }

    private Transform GetCurrentPanel()
    {
        return currentTab switch
        {
            ShopTab.Vehicle => vehiclePanel,
            ShopTab.Consumable => consumablePanel,
            _ => vehiclePanel
        };
    }

    private bool IsItemInCurrentTab(PerformanceItemSO item)
    {
        return currentTab switch
        {
            ShopTab.Vehicle => item.itemType == ItemType.Permanent || item.itemType == ItemType.Vehicle,
            ShopTab.Consumable => item.itemType == ItemType.Consumable,
            _ => false,
        };
    }

    public void BuySelectedItem(PerformanceItemSO item)
    {
        if (PerformanceInventoryManager.Instance.IsOwned(item)) return;
        if (GameDataManager.Instance.data.money < item.price)
        {
            Debug.Log("❌ 골드 부족");
            return;
        }

        GameDataManager.Instance.data.money -= item.price;
        PerformanceInventoryManager.Instance.BuyItem(item);
        UpdateMoneyUI();
        RefreshAllSlots();
    }

    public void EquipSelectedItem(PerformanceItemSO item)
    {
        PerformanceInventoryManager.Instance.EquipItem(item.category, item);
        RefreshAllSlots();
    }

    private void RefreshAllSlots()
    {
        Transform targetPanel = GetCurrentPanel();
        PerformanceInventoryManager.Instance.LoadFromGameData(GameDataManager.Instance.data);

        foreach (Transform child in targetPanel)
        {
            if (child.TryGetComponent(out PerformanceItemSlot slot))
                slot.Refresh();
        }
    }

    private void UpdateMoneyUI()
    {
        moneyText.text = GameDataManager.Instance.data.money + "원";
    }

    private void UpdateTurnAndPaymentUI()
    {
        int remainingTurns = totalTurnsPerRound - (GameDataManager.Instance.data.turn % totalTurnsPerRound);
        string color = remainingTurns <= 1 ? "#FF5555" : "#55FF55";

        turnStatusText.text = $"<color={color}>남은 턴: {remainingTurns} / {totalTurnsPerRound}</color>";
        paymentAmountText.text = $"집세 : {GameDataManager.Instance.GetRequiredPayment()}원";
    }

    public void TryPayNextStage()
    {
        bool success = GameDataManager.Instance.TryPay();
        if (success)
        {
            UpdateMoneyUI();
            UpdateTurnAndPaymentUI();
        }
        else
        {
            Debug.Log("❌ 돈이 부족합니다.");
        }
    }

    // ==============================
    // 👇 일회성 아이템 관련
    // ==============================

    public void OnGameStartButtonClicked()
    {
        // 다른 탭 패널들 숨기기
        vehiclePanel.gameObject.SetActive(false);
        consumablePanel.gameObject.SetActive(false);

        // 일회성 패널 생성 및 보여주기
        ShowOneTimeItemSelection();
        oneTimePanel.gameObject.SetActive(true);
    }

    public void OnOneTimeConfirmButtonClicked()
    {
        ApplySelectedOneTimeItems();
        oneTimePanel.gameObject.SetActive(false);

        StartCoroutine(DelayedStartGame());
    }

    private IEnumerator DelayedStartGame()
    {
        yield return null;
        Debug.Log("🎮 게임 시작!");
        // SceneManager.LoadScene("GameScene");
    }

    private void ShowOneTimeItemSelection()
    {
        foreach (Transform child in oneTimePanel)
            Destroy(child.gameObject);

        foreach (var item in allItems)
        {
            if (item.itemType != ItemType.OneTime) continue;

            var slot = Instantiate(oneTimeSlotPrefab, oneTimePanel);
            if (slot.TryGetComponent(out PerformanceOneTimeSlot oneTimeSlot))
                oneTimeSlot.Setup(item);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(oneTimePanel.GetComponent<RectTransform>());
    }

    public void ApplySelectedOneTimeItems()
    {
        var oneTimeSlots = FindObjectsOfType<PerformanceOneTimeSlot>(true);
        foreach (var slot in oneTimeSlots)
        {
            if (!slot.IsSelected) continue;

            var data = slot.GetItemData();
            if (GameDataManager.Instance.data.money >= data.price)
            {
                GameDataManager.Instance.data.money -= data.price;
                GameDataManager.Instance.data.ownedItems.Add(new SerializableItem
                {
                    itemId = data.name,
                    itemType = ItemType.OneTime,
                    count = 1,
                    isUnlocked = true,
                    isEquipped = false
                });
                Debug.Log($"✅ {data.DisplayName} 선택됨 - {data.price}원 차감됨");
            }
        }

        GameDataManager.Instance.Save();
        UpdateMoneyUI();
    }

    private GameObject GetPrefabForItem(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Consumable => consumableSlotPrefab,
            ItemType.OneTime => oneTimeSlotPrefab,
            _ => vehicleSlotPrefab
        };
    }
}
