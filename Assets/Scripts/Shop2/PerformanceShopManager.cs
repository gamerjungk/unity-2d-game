using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PerformanceShopManager : MonoBehaviour
{
    public static PerformanceShopManager Instance { get; private set; }

    [Header("UI References")]
    public TextMeshProUGUI moneyText;
    public Transform shopPanel;
    public GameObject performanceItemSlotPrefab;
    [SerializeField] private Button payButton; // 💸 집세 납부 버튼

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
        UpdateMoneyUI();

        // 💸 납부 버튼 이벤트 연결
        payButton.onClick.AddListener(TryPayNextStage);
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
}
