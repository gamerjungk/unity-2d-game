using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image itemImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public Color yellowColor = Color.yellow;
    public Color greenColor = Color.green;
    public Color redColor = Color.red;
    public Color grayColor = Color.gray;
    public Color whiteColor = Color.white;
    
    public Button buyButton;
    public TextMeshProUGUI ownedText;

    private ItemSO itemData;

    public void Setup(ItemSO data)
    {
        itemData = data;
        int ownedCount = PlayerInventory.GetCount(data);

        itemImage.sprite = data.image;
        itemNameText.text = data.itemName;
        ownedText.text = $"보유: {ownedCount}/{data.maxCount}";

        buyButton.onClick.RemoveAllListeners();

        if (ownedCount >= data.maxCount)
        {   
            ownedText.color = redColor; // ✅ 여기가 빨간색으로 고정되어야 함

            itemPriceText.text = "보유중";
            itemPriceText.color = grayColor;
            buyButton.interactable = false;
        }
        else
        {
            // 1개 이상 보유 시 초록색, 아니면 흰색
            if (ownedCount >= 1)
                ownedText.color = greenColor;
            else
                ownedText.color = whiteColor;

            itemPriceText.text = $"{data.price}G";
            itemPriceText.color = yellowColor;
            buyButton.interactable = true;
            buyButton.onClick.AddListener(() => ShopManager.Instance.BuyItem(itemData));
        }


    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string tooltipMessage = $"<b>{itemData.itemName}</b>\n<size=80%>{itemData.description}</size>";
        TooltipManager.Instance.ShowTooltip(tooltipMessage);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideTooltip();
    }
}