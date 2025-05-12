using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

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

    public ItemSO[] items;

    private ItemSO pendingItem;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        popupPanel.SetActive(false);
        confirmPopupPanel.SetActive(false);
        UpdateGoldUI();
        GenerateShopSlots();
    }

    private void GenerateShopSlots()
    {
        Dictionary<string, List<ItemSO>> categoryGroups = new Dictionary<string, List<ItemSO>>();
        foreach (var item in items)
        {
            if (!categoryGroups.ContainsKey(item.category))
                categoryGroups[item.category] = new List<ItemSO>();
            categoryGroups[item.category].Add(item);
        }

        foreach (var group in categoryGroups)
        {
            GameObject categoryGroup = Instantiate(categoryGroupPrefab, shopPanel);
            categoryGroup.transform.Find("CategoryTitle").GetComponent<TextMeshProUGUI>().text = group.Key;
            Transform itemContainer = categoryGroup.transform.Find("ItemContainer");

            foreach (var item in group.Value)
            {
                GameObject slot = Instantiate(itemSlotPrefab, itemContainer);
                slot.GetComponent<ItemSlot>().Setup(item);
                Debug.Log("슬롯 생성 성공: " + item.itemName);
            }
            
        }
        if (bottomSpacerPrefab != null)
            Instantiate(bottomSpacerPrefab, shopPanel);
    }

    
    
    public void BuyItem(ItemSO item)
    {
        if (PlayerInventory.GetCount(item) >= item.maxCount)
        {
            ShowPopup("이미 최대 보유 수량입니다.");
            return;
        }

        if (GameManager.gold < item.price)
        {
            ShowPopup("골드가 부족합니다!");
            return;
        }

        pendingItem = item;
        confirmPopupPanel.SetActive(true);
        confirmMessageText.text = $"정말로 '{item.itemName}'을(를) 구매하시겠습니까?";

        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => ConfirmPurchase());
        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(() => CancelPurchase());
    }

    private void ConfirmPurchase()
    {
        GameManager.gold -= pendingItem.price;
        PlayerInventory.AddItem(pendingItem);
        UpdateGoldUI();
        RefreshShopSlots();
        confirmPopupPanel.SetActive(false);
    }

    private void CancelPurchase()
    {
        confirmPopupPanel.SetActive(false);
    }

    private void RefreshShopSlots()
    {
        foreach (Transform child in shopPanel)
        {
            Destroy(child.gameObject);
        }

        // 레이아웃 깨짐 방지용 - 레이아웃 강제 업데이트
        Canvas.ForceUpdateCanvases();

        GenerateShopSlots();

        // 재배치 직후 다시 강제 레이아웃 계산
        LayoutRebuilder.ForceRebuildLayoutImmediate(shopPanel.GetComponent<RectTransform>());
    }


    private void ShowPopup(string message)
    {
        popupText.text = message;
        popupPanel.SetActive(true);
    }

    public void ClosePopup()
    {
        popupPanel.SetActive(false);
    }

    private void UpdateGoldUI()
    {
        goldText.text = GameManager.gold.ToString() + "G";
    }
}