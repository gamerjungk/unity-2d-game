using UnityEngine;

// 속도, 연비, 적재량 등의 성능 수치를 가진 특수 아이템용 ScriptableObject
[CreateAssetMenu(menuName = "Shop/PerformanceItem")]    // 우클릭 메뉴 생성 경로
public class PerformanceItemSO : ScriptableObject
{
    [Header("고유 ID (예: car1, scooter2)")]
    public string itemId;   // 각 아이템을 식별하기 위한 고유 ID (저장 및 비교용)

    public Sprite image;    // 아이템 이미지 (UI에 표시할 아이콘)
    public PerformanceCategorySO category;  // 아이템 분류 (예: 차량, 바이크 등) - 별도 SO 관리
    public int price;       // 구매 가격
    public float speed;     // 속도 수치
    public float efficiency;   // 연비 수치
    public float capacity;     // 적재량 수치

    public ItemType itemType;   // 아이템 종류(enum) - Consumable, OneTime 등

    // 다국어 지원용 이름 및 설명
    public string itemNameKR;   // 한국어 이름
    public string itemNameEN;   // 영어 이름

    [TextArea] public string descriptionKR; // 한국어 설명
    [TextArea] public string descriptionEN; // 영어 설명

    // 현재 언어 설정에 따라 적절한 이름 반환
    public string DisplayName =>
        LocalizationManager.Instance.currentLanguage == Language.Korean ? itemNameKR : itemNameEN;

    // 현재 언어 설정에 따라 적절한 설명 반환
    public string DisplayDescription =>
        LocalizationManager.Instance.currentLanguage == Language.Korean ? descriptionKR : descriptionEN;
}
