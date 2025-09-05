using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotPerf : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public TextMeshProUGUI ownedText;
    public Button buyButton;

    PerformanceItemSO data;

    public void Setup(PerformanceItemSO so)
    {
        data = so;
        itemImage.sprite = so.image;
        itemNameText.text = so.itemNameKR;  // 간단히 KR 사용
        itemPriceText.text = $"{so.price}원";

        bool owned = PerformanceInventoryManager.Instance.ownedItemIds.Contains(so.itemId);
        ownedText.text = owned ? "보유중" : "미보유";

        buyButton.onClick.RemoveAllListeners();
        buyButton.interactable = GameDataManager.Instance.data.money >= so.price && !owned;
        buyButton.onClick.AddListener(Buy);
    }

    void Buy()
    {
        var gdm = GameDataManager.Instance;
        if (gdm.data.money < data.price) return;

        // 돈 차감
        gdm.SubMoney(data.price);                        // Money 감소
        // 퍼포먼스 인벤토리에 기록(+ 세이브)
        PerformanceInventoryManager.Instance.BuyItem(data); // ownedItems/ownedItemIds 및 GameData 동기화
        // UI 새로고침
        FindAnyObjectByType<ShopManagerPerf>()?.RefreshAll();
    }
}
