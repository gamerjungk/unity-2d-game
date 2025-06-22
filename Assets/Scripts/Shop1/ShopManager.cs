using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }    // 싱글톤 인스턴스

    // UI 요소 참조
    public TextMeshProUGUI goldText;
    public GameObject popupPanel;
    public TextMeshProUGUI popupText;
    public Transform shopPanel;
    public GameObject itemSlotPrefab;
    public GameObject categoryGroupPrefab;
    public GameObject confirmPopupPanel;
    public TextMeshProUGUI confirmMessageText;
    public Button yesButton;
    public Button noButton;
    public GameObject bottomSpacerPrefab;

    public ItemSO[] items;  // 상점에 표시할 아이템 목록

    private ItemSO pendingItem; // 구매 대기 중인 아이템

    private void Awake()
    {   
        // 싱글톤 설정
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void OnEnable()
    {   
        // 데이터 로드 이벤트 구독
        GameDataManager.OnDataLoaded += OnDataReady;
        GameDataManager.OnDataReloaded += OnDataReady;

        // 이미 초기화되었으면 직접 호출
        if (GameDataManager.Instance != null && GameDataManager.Instance.IsInitialized)
        {
            OnDataReady();
        }
    }

    private void OnDisable()
    {   
        // 이벤트 구독 해제
        GameDataManager.OnDataLoaded -= OnDataReady;
        GameDataManager.OnDataReloaded -= OnDataReady;
    }

    // 데이터 로딩이 완료되었을 때 호출
    private void OnDataReady()
    {
        PlayerInventory.LoadFromGameData(GameDataManager.Instance.data);    // 인벤토리 로드
        UpdateGoldUI(); // 골드 표시 업데이트
        RefreshShopSlots(); // 상점 슬롯 새로 생성
    }

    private void Start()
    {
        // 팝업 비활성화
        popupPanel.SetActive(false);
        confirmPopupPanel.SetActive(false);
    }

    // 상점 슬롯 생성
    private void GenerateShopSlots()
    {
        Dictionary<string, List<ItemSO>> categoryGroups = new Dictionary<string, List<ItemSO>>();
        foreach (var item in items)
        {   
            // 카테고리별로 그룹화
            if (!categoryGroups.ContainsKey(item.category))
                categoryGroups[item.category] = new List<ItemSO>();
            categoryGroups[item.category].Add(item);
        }

        // 각 카테고리에 대한 슬롯 생성
        foreach (var group in categoryGroups)
        {
            GameObject categoryGroup = Instantiate(categoryGroupPrefab, shopPanel); // 카테고리 그룹 생성
            categoryGroup.transform.Find("CategoryTitle").GetComponent<TextMeshProUGUI>().text = group.Key;
            Transform itemContainer = categoryGroup.transform.Find("ItemContainer");

            foreach (var item in group.Value)
            {
                GameObject slot = Instantiate(itemSlotPrefab, itemContainer);   // 아이템 슬롯 생성
                slot.GetComponent<ItemSlot>().Setup(item);  // 슬롯 초기화
                Debug.Log("슬롯 생성 성공: " + item.itemName);
            }
        }
        // 하단 스페이서 추가 (스크롤 여백용)
        if (bottomSpacerPrefab != null)
            Instantiate(bottomSpacerPrefab, shopPanel);
    }

    // 아이템 구매 시 호출
    public void BuyItem(ItemSO item)
    {   
        // 최대 보유 수량 초과 시
        if (PlayerInventory.GetCount(item) >= item.maxCount)
        {
            ShowPopup("이미 최대 보유 수량입니다.");
            return;
        }
    
        // 골드 부족 시
        if (GameDataManager.Instance.data.gold < item.price)
        {
            ShowPopup("골드가 부족합니다!");
            return;
        }

        // 구매 확인 팝업 준비
        pendingItem = item;
        confirmPopupPanel.SetActive(true);
        confirmMessageText.text = $"정말로 '{item.itemName}'을(를) 구매하시겠습니까?";

        // 버튼 이벤트 설정
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => ConfirmPurchase());
        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(() => CancelPurchase());
    }

    // 실제 구매 처리
    private void ConfirmPurchase()
    {
        GameDataManager.Instance.data.gold -= pendingItem.price;    // 골드 차감
        PlayerInventory.AddItem(pendingItem);   // 아이템 추가
        GameDataManager.Instance.Save();    // 저장
        UpdateGoldUI(); // 골드 UI 갱신
        RefreshShopSlots(); // 상점UI 새로고침
        confirmPopupPanel.SetActive(false); // 팝업 닫기
    }

    // 구매 확인 팝업 닫기
    private void CancelPurchase()
    {
        confirmPopupPanel.SetActive(false);
    }

    // 상점 슬롯 초기화 및 재생성
    private void RefreshShopSlots()
    {
        foreach (Transform child in shopPanel)
        {
            Destroy(child.gameObject);
        }

        Canvas.ForceUpdateCanvases();
        GenerateShopSlots();
        LayoutRebuilder.ForceRebuildLayoutImmediate(shopPanel.GetComponent<RectTransform>());
    }

    // 경고 및 안내 팝업 메시지 표시
    private void ShowPopup(string message)
    {
        popupText.text = message;
        popupPanel.SetActive(true);
    }

    // 경고 및 안내 팝업 닫기
    public void ClosePopup()
    {
        popupPanel.SetActive(false);
    }

    // 골드 텍스트 갱신
    private void UpdateGoldUI()
    {
        goldText.text = GameDataManager.Instance.data.gold.ToString() + "G";
    }
}
