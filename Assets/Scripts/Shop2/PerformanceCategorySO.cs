using UnityEngine;

[CreateAssetMenu(menuName = "Shop/PerformanceCategory")]
public class PerformanceCategorySO : ScriptableObject
{
    public string categoryName;
    public string description;
    public bool allowMultipleEquip = false;   // 해당 카테고리에서 여러 개 장착 가능 여부
    public bool allowMultipleEffect = false;  // 해당 카테고리에서 효과 중첩 허용 여부
}