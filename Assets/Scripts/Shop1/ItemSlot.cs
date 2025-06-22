using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

// 상점의 아이템 슬롯 하나를 관리하는 클래스. 마우스 오버 시 툴팁 표시 및 구매 버튼 처리 포함
public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image itemImage;                     // 아이템 이미지
    public TextMeshProUGUI itemNameText;        // 아이템 이름 텍스트
    public TextMeshProUGUI itemPriceText;       // 아이템 가격 텍스트
    public Color yellowColor = Color.yellow;    // 가격 텍스트용 색상
    public Color greenColor = Color.green;      // 보유 텍스트(보유 중) 색상
    public Color redColor = Color.red;          // 보유 텍스트(최대 보유 시) 색상
    public Color grayColor = Color.gray;        // 가격 텍스트(구매 불가 시) 색상
    public Color whiteColor = Color.white;      // 보유 텍스트 기본 색상

    public Button buyButton;                    // 구매 버튼
    public TextMeshProUGUI ownedText;           // 보유 개수 텍스트

    private ItemSO itemData;                    // 현재 슬롯에 표시 중인 아이템 데이터

    // 슬롯에 아이템 데이터를 바인딩하는 함수
    public void Setup(ItemSO data)
    {
        itemData = data;
        int ownedCount = PlayerInventory.GetCount(data);    // 보유 개수 확인

        itemImage.sprite = data.image;
        itemNameText.text = data.itemName;
        ownedText.text = $"보유: {ownedCount}/{data.maxCount}";

        buyButton.onClick.RemoveAllListeners(); // 리스너 중복 방지

        if (ownedCount >= data.maxCount)    // 최대 보유 시
        {
            ownedText.color = redColor; // 글자를 빨간색으로

            itemPriceText.text = "보유중";
            itemPriceText.color = grayColor;
            buyButton.interactable = false; // 구매 불가능
        }
        else
        {
            // 최대 개수만큼 보유중이 아니고, 보유 중이면 초록, 아니면 흰색으로 표시
            if (ownedCount >= 1)
                ownedText.color = greenColor;
            else
                ownedText.color = whiteColor;

            itemPriceText.text = $"{data.price}G";
            itemPriceText.color = yellowColor;
            buyButton.interactable = true;

            // 구매 버튼 클릭 시 ShopManager로 구매 요청
            buyButton.onClick.AddListener(() => ShopManager.Instance.BuyItem(itemData));
        }


    }

    // 마우스 포인터가 슬롯 위에 올라갔을 때 툴팁을 보여줌
    public void OnPointerEnter(PointerEventData eventData)
    {
        string tooltipMessage = $"<b>{itemData.itemName}</b>\n<size=80%>{itemData.description}</size>";
        TooltipManager.Instance.ShowTooltip(tooltipMessage);
    }

    // 마우스 포인터가 슬롯에서 벗어났을 때 툴팁이 사라짐
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideTooltip();
    }
}