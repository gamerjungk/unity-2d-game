using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public TextMeshProUGUI goldText;              // 골드 표시 UI
    public GameObject popupPanel;                 // 팝업 창 (Panel)
    public TextMeshProUGUI popupText;             // 팝업 메시지 텍스트
    public Transform shopPanel;                   // 아이템 슬롯 (UI 패널)
    public GameObject itemSlotPrefab;             // 아이템 슬롯 프리펩
    
    private int currentGold;

    [System.Serializable]
    public class ItemData                         // 아이템 정보를 담을 구조체
    {
        public Sprite image;
        public string name;
        public int price;
    }

    public ItemData[] items;                      // 아이템 정보를 저장할 배열

    private void Start()
    {
        currentGold = GameManager.gold;          // 게임 외적재화를 어디서 관리하게 될지 몰라서 일단 Gamemanager 임의로 만들어서 넣엇음..
        UpdateGoldUI(); // 시작 시 UI 초기화         // 현재 정보를 토대로 UI 업데이트
        popupPanel.SetActive(false);             // 에러 메시지 팝업 비활성화 (골드 부족 등..)

        GenerateShopSlots();                    // 아이템 슬롯 생성 (화면에)
    }

        private void GenerateShopSlots()
    {
        foreach (var item in items)             // 등록된 아이템 개수만큼 아이템 슬롯 생성
        {
            GameObject slot = Instantiate(itemSlotPrefab, shopPanel);   // 슬롯 프리팹 만들어서 상점 패널에 배치.
            slot.GetComponent<ItemSlot>().Setup(item.image, item.name, item.price);   // 새로 만든 슬롯에 아이템 정보 초기화.
        }
    }

    private void UpdateGoldUI()
    {
        goldText.text = GameManager.gold.ToString() + "G";
    }

    // 아이템 구매 (골드 차감)
    public void BuyItem(int price)
    {
        if (GameManager.gold >= price)
        {
            GameManager.gold -= price;
            Debug.Log($"아이템 구매! {price} 골드 차감");
            UpdateGoldUI(); // UI 업데이트
        }
        else
        {
            ShowPopup("골드가 부족합니다!");
        }
    }

    // 아이템 판매 (골드 추가)
    public void SellItem(int price)
    {
        GameManager.gold += price;
        Debug.Log($"아이템 판매! {price} 골드 획득");
        UpdateGoldUI(); // 골드 UI 업데이트
    }

    // 팝업 창 표시
    private void ShowPopup(string message)
    {
        popupText.text = message;
        popupPanel.SetActive(true); // 팝업 창 활성화
    }

    // 팝업 창 닫기 (버튼에서 호출)
    public void ClosePopup()
    {
        popupPanel.SetActive(false);
    }
}
