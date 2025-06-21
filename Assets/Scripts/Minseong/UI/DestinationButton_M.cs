using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DestinationButton_M : MonoBehaviour
{
    // 버튼에 표시할 TextMeshPro 텍스트 컴포넌트 참조
    [SerializeField] TMP_Text label;

    // 색상 추가(3줄)
    [Header("Text Colors")] // 헤더 표시
    [SerializeField] Color pickupColor = Color.black; // 픽업 상태일 때 텍스트 색상 (기본: 검정)
    [SerializeField] Color deliveryColor = new(0.12f, 0.55f, 1f); // 배달 상태일 때 텍스트 색상(파랑색)

    int index; // 이 버튼이 가리키는 인덱스 저장 변수
    DestinationUI_M ui; // 상위 DestinationUI 관리 스크립트 참조
 
    /* 초기화 */
    public void Init(int idx, DestinationUI_M parent)
    {
        index = idx; // 전달된 인덱스로 설정
        ui = parent; // 부모 UI 스크립트 참조 저장
        GetComponent<Button>() // Button 컴포넌트를 가져오기
            .onClick.AddListener(() => ui.SelectIndex(index)); // 클릭 시 부모 UI에 버튼의 인덱스 전달
    }

    public void SetLabel(string text)
    {
        label.text = text; // 전달된 문자열로 버튼 라벨 텍스트 설정
    }

    // 픽업/배달에 따른 색 변경 추가
    public void SetAsPickup() => label.color = pickupColor; // 픽업 상태일 때 색상 적용
    public void SetAsDelivery() => label.color = deliveryColor; // 배달 상태일 때 색상 적용

}
