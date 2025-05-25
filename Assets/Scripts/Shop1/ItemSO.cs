using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Item")]
public class ItemSO : ScriptableObject
{
    public string category;
    public Sprite image;
    public string itemName;
    public int price;
    [TextArea] public string description;
    public int maxCount = 1;
}