using UnityEngine;
using TMPro;

public class InventoryItemSlot : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI countText;

    public void Setup(SerializableItem item)
    {
        nameText.text = item.itemId;
        countText.text = $"x{item.count}";
    }
}
