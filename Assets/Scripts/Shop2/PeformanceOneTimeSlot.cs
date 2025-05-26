using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PerformanceOneTimeSlot : MonoBehaviour
{
    [Header("UI")]
    public Image itemImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI priceText;
    public Toggle checkToggle;

    private PerformanceItemSO itemData;

    public bool IsSelected => checkToggle.isOn; // 외부에서 상태 확인용

    public void Setup(PerformanceItemSO data)
    {
        itemData = data;

        itemImage.sprite = itemData.image;
        itemNameText.text = itemData.DisplayName;
        priceText.text = $"{itemData.price}원";

        // 💰 돈이 부족하면 토글 비활성화
        if (GameDataManager.Instance.data.money < itemData.price)
        {
            checkToggle.isOn = false;
            checkToggle.interactable = false;
        }
        else
        {
            checkToggle.interactable = true;
        }

        // 항상 초기에는 체크 해제 상태
        checkToggle.isOn = false;
    }

    public PerformanceItemSO GetItemData()
    {
        return itemData;
    }
}
