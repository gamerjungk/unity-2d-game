using UnityEngine;

// 준비화면에 표시될 기본 아이템 정보를 담는 ScriptableObject
[CreateAssetMenu(menuName = "Shop/Item")]   // 우클릭 메뉴 생성 경로 지정
public class ItemSO : ScriptableObject
{
    public string category; // 카테고리 이름 (예: 스킨, 패시브 아이템 등)
    public Sprite image;    // 아이템 아이콘 이미지
    public string itemName; // 아이템 이름 (한 언어만 사용하는 단순 구조)
    public int price;       // 구매 가격
    [TextArea] public string description;   // 아이템 설명 텍스트 (UI용)
    public int maxCount = 1;    // 최대 보유 수량 (기본값 1)
}