using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 일회성 아이템 슬롯 UI를 구성하는 스크립트
public class PerformanceOneTimeSlot : MonoBehaviour
{
    [Header("UI")]
    public Image itemImage;                 // 아이템 이미지
    public TextMeshProUGUI itemNameText;    // 아이템 이름 텍스트
    public TextMeshProUGUI priceText;       // 가격 텍스트
    public Toggle checkToggle;              // 선택 여부를 표시하는 토글

    private PerformanceItemSO itemData;     // 아이템 데이터 참조

    // 외부에서 선택 여부를 확인할 수 있는 속성
    public bool IsSelected => checkToggle.isOn;

    // 슬롯에 데이터를 세팅하고 UI를 갱신하는 함수
    public void Setup(PerformanceItemSO data)
    {
        itemData = data;                            // 아이템 데이터 저장

        itemImage.sprite = itemData.image;          // 이미지 설정
        itemNameText.text = itemData.DisplayName;   // 이름 설정
        priceText.text = $"{itemData.price}원";     // 가격 표시

        // 돈이 부족하면 토글 비활성화
        if (GameDataManager.Instance.data.money < itemData.price)
        {
            checkToggle.isOn = false;
            checkToggle.interactable = false;
        }
        else
        {
            checkToggle.interactable = true;
        }

        // // 항상 기본은 선택 해제 상태
        checkToggle.isOn = false;
    }

    // 해당 슬롯에 연결된 아이템 데이터를 반환
    public PerformanceItemSO GetItemData()
    {
        return itemData;
    }
}
