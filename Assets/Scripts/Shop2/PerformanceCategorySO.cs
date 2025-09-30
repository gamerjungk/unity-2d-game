using UnityEngine;

// 성능 아이템의 카테고리 정보를 정의하는 ScriptableObject
[CreateAssetMenu(menuName = "Shop/PerformanceCategory")]
public class PerformanceCategorySO : ScriptableObject   // 우클릭 메뉴 경로 설정
{
    public string categoryName; // 카테고리 이름 (예: 차량, 엔진, 휠 등)
    public string description;  // 카테고리에 대한 설명 (툴팁 등 UI 표시용)
    public bool allowMultipleEquip = false;     // 해당 카테고리에서 여러 개 장착 허용 여부
                                                // true: 동시에 여러 아이템 장착 가능 (예: 보조장치)
                                                // false: 1개만 장착 가능 (예: 차량 1대만)
    public bool allowMultipleEffect = false;    // 해당 카테고리 내 아이템 효과 중첩 허용 여부
                                                // true: 여러 효과가 누적됨
                                                // false: 마지막 장착 아이템만 효과 적용
}