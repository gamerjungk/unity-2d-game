// PerformanceItemSO.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Shop/PerformanceItem")]
public class PerformanceItemSO : ScriptableObject
{
    public Sprite image;
    public string itemName;
    public PerformanceCategorySO category;
    public int price;
    public string description;
    public float speed;
    public float efficiency;
    public float capacity;
}