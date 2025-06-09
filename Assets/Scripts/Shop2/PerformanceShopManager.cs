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
    private bool payButtonAssigned = false;
    [Header("게임 시작 버튼들")]
    [SerializeField] private GameObject gamePrepareButton;
    [SerializeField] private GameObject gameStartButton;

    [Header("아이템 데이터")]
    public PerformanceItemSO[] allItems;

    private ShopTab currentTab = ShopTab.Vehicle;
    private const int totalTurnsPerRound = 5;
    private bool isSubscribed = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        payButton.onClick.RemoveAllListeners(); // 💡 중복 방지
        payButton.onClick.AddListener(() => TryPayNextStage()); // ✅ 람다로 고정
    }

    private void OnEnable()
    {
        if (!isSubscribed)
        {
            GameDataManager.OnDataLoaded += OnGameDataReady;
            PerformanceInventoryManager.OnInventoryLoaded += RefreshAllSlots;
            isSubscribed = true;
        }
        if (GameDataManager.Instance != null && GameDataManager.Instance.IsInitialized)
        {
            Debug.Log("⚠️ OnDataLoaded 이벤트 놓쳐서 수동으로 호출");
            OnGameDataReady();
        }

        // 리스너는 오직 1번만 등록되도록 방지
        if (!payButtonAssigned)
        {
            payButton.onClick.RemoveAllListeners(); // 혹시 남아있는 걸 제거
            payButtonAssigned = true;
        }

        gamePrepareButton.SetActive(true);
        gameStartButton.SetActive(false);
    }

    private void OnDisable()
    {
        if (isSubscribed)
        {
            GameDataManager.OnDataLoaded -= OnGameDataReady;
            PerformanceInventoryManager.OnInventoryLoaded -= RefreshAllSlots;
            isSubscribed = false;
        }
    }

    private void OnGameDataReady()
    {
        Debug.Log("🚨 OnGameDataReady 호출됨");
        vehiclePanel.gameObject.SetActive(true);
        consumablePanel.gameObject.SetActive(false);
        oneTimePanel.gameObject.SetActive(false);

        UpdateMoneyUI();
        UpdateTurnAndPaymentUI();
        GenerateShopSlots();
    }

    public void OnTabSelected(int tabIndex)
    {
        currentTab = (ShopTab)tabIndex;

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
            Debug.Log($"[CHECK] item.name = {item.name}, itemId = {item.itemId}, isOwned: {PerformanceInventoryManager.Instance.IsOwned(item)}");
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
                        var ownedItem = GameDataManager.Instance.data.ownedItems.Find(x => x.itemId == item.itemId);
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
        Debug.Log("🔁 RefreshAllSlots 호출됨");

        Transform targetPanel = GetCurrentPanel();

        foreach (Transform child in targetPanel)
        {
            if (child.TryGetComponent(out PerformanceItemSlot slot))
                slot.Refresh();
        }
    }

    public void UpdateMoneyUI()
    {
        moneyText.text = GameDataManager.Instance.data.money + "원";
    }

    public void UpdateTurnAndPaymentUI()
    {
        int remainingTurns = totalTurnsPerRound - (GameDataManager.Instance.data.turn % totalTurnsPerRound);
        string color = remainingTurns <= 1 ? "#FF5555" : "#55FF55";

        turnStatusText.text = $"<color={color}>남은 턴: {remainingTurns} / {totalTurnsPerRound}</color>";
        paymentAmountText.text = $"집세 : {GameDataManager.Instance.GetRequiredPayment()}원";
    }

    public void TryPayNextStage()
    {
        Debug.Log($"[TryPayNextStage] 호출됨 - money: {GameDataManager.Instance.data.money}");

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


    public void OnGameStartButtonClicked()
    {
        vehiclePanel.gameObject.SetActive(false);
        consumablePanel.gameObject.SetActive(false);

        ShowOneTimeItemSelection();
        oneTimePanel.gameObject.SetActive(true);
    }
    public void OnOneTimeConfirmButtonClicked()
    {
        ApplySelectedOneTimeItems();

        // 시작 버튼도 숨기기
        gameStartButton.SetActive(false);
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
        var oneTimeSlots = Object.FindObjectsByType<PerformanceOneTimeSlot>(FindObjectsSortMode.None);

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
    private void OnDataReloadedExternally()
    {
        Debug.Log("🔄 외부에서 데이터가 갱신됨, 상점 UI 다시 생성");
        OnGameDataReady(); // 전체 다시 초기화해서 UI 재생성
    }
    
    public void OnGamePrepareButtonClicked()
    {
        gamePrepareButton.SetActive(false);  // 준비 버튼 숨김
        gameStartButton.SetActive(true);     // 시작 버튼 표시

        // 선택 UI 표시
        vehiclePanel.gameObject.SetActive(false);
        consumablePanel.gameObject.SetActive(false);
        ShowOneTimeItemSelection();
        oneTimePanel.gameObject.SetActive(true);
    }

}
