using UnityEngine;
using UnityEngine.UI;
using TMPro;  // TextMeshPro를 쓸 경우

public class InventoryUI : MonoBehaviour
{
    public Transform inventoryPanel;
    public GameObject inventorySlotPrefab;

    public void RefreshInventory()
    {
        foreach (Transform child in inventoryPanel)
            Destroy(child.gameObject);

        foreach (var item in GameDataManager.Instance.data.ownedItems)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryPanel);
            slot.GetComponent<InventoryItemSlot>().Setup(item);
        }
    }
}
