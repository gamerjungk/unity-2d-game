using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image itemImage;                 // 아이템 이미지 UI
    public TextMeshProUGUI itemNameText;    // 아이템 이름
    public TextMeshProUGUI itemPriceText;   // 아이템 가격
    public Button buyButton;                // 구매버튼 

    private int itemPrice;

    public void Setup(Sprite image, string name, int price)     // 아이템 이미지, 이름, 가격을 받아서 초기화 시켜줌. GameManager에 넣을필요는 없을거같음.
    {
        itemImage.sprite = image;
        itemNameText.text = name;
        itemPriceText.text = price + "G";
        itemPrice = price;

        buyButton.onClick.AddListener(() => BuyItem());         // 구매버튼 딸각 = 아이템 구매.
    }

    private void BuyItem()
    {
        FindObjectOfType<ShopManager>().BuyItem(itemPrice);     // 샵매니저에 아이템 가격 전달 나머지는 ShopManager 스크립트에서 처리.
    }
}

