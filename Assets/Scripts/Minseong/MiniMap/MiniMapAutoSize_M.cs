using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class MiniMapAutoSize_M : MonoBehaviour
{
    [Header("비율 설정")] // 헤더 표시
    [Tooltip("가로 화면 기준 % (ex) 0.25 => 화면 폭의 25 %")]
    [Range(0.05f, 0.5f)] public float widthPercent = 0.25f; // 미니맵 너비 비율 설정
    [Tooltip("세로 화면 기준 % (ex) 0.14 => 화면 높이의 14 %")]
    [Range(0.05f, 0.5f)] public float heightPercent = 0.14f; // 미니맵 높이 비율 설정

    [Header("겹침 방지 대상")] // 헤더 표시
    [Tooltip("cur / Turn 등의 상단 라벨 RectTransform (없으면 무시)")]
    public RectTransform avoidLabel; // 미니맵이 겹치지 않게 피할 UI 라벨

    [Header("여유 간격")] // 헤더 표시
    public float marginPixel = 8f; // 라벨과 미니맵 사이의 최소 여유 간격

    public float borderThickness = 8f; // 미니맵 테두리 두께

    RectTransform rt; // RectTransform 참조
    CanvasScaler scaler; // 부모 CanvasScaler 참조

    void Awake()
    {
        rt = GetComponent<RectTransform>(); // RectTransform 컴포넌트 가져오기
        ApplySize(); // 초기 크기 및 위치 적용
    }

#if UNITY_EDITOR        // 에디터에서 값 변경되면 즉시 반영
    void OnValidate()
    {
        if (!rt) rt = GetComponent<RectTransform>(); // rt = null이면 다시 할당
        if (!scaler) scaler = GetComponentInParent<CanvasScaler>(); // scaler = null이면 부모에서 찾기
        ApplySize(); // 변경된 값으로 크기 및 위치 재적용
    }
#endif

    void ApplySize()
    {
        /* ─── 1. 기본 크기 계산 ─── */
        Vector2 refRes = scaler                              // CanvasScaler가 있으면
                         ? scaler.referenceResolution        //   기준 해상도 사용
                         : new Vector2(Screen.width, Screen.height); // 없으면 실제 화면 크기로 대체

        float targetW = refRes.x * widthPercent;             // 목표 너비(pixel) 계산
        float targetH = refRes.y * heightPercent;            // 목표 높이(pixel) 계산

        /* ─── 2. 라벨과 겹치지 않게 위치 보정 ─── */
        if (avoidLabel)                                       // avoidLabel이 설정되어 있으면
        {
            var wc = new Vector3[4];                         // 사각형 네 모서리 좌표 저장할 배열
            avoidLabel.GetWorldCorners(wc);                  // 라벨의 월드 공간 모서리 좌표 가져오기
            float labelBottom = RectTransformUtility          // 월드 좌표 중 아래쪽 모서리를
                                .WorldToScreenPoint(null, wc[0]).y; // 스크린 Y 좌표로 변환

            float mapTop = targetH + marginPixel;            // 미니맵 상단 Y 위치 계산
            if (mapTop > labelBottom)                        // 라벨 아래로 침범하면
            {
                float offset = mapTop - labelBottom;         // 겹치는 만큼 오프셋 계산
                rt.anchoredPosition -= new Vector2(0, offset); // 미니맵을 아래로 이동
            }
        }

        /* ─── 3. 크기 및 내부 보더 적용 ─── */
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetW); // RectTransform의 가로 크기 설정
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetH); // RectTransform의 세로 크기 설정

        if (rt.childCount > 0)                         // 안쪽 Raw-Image 조절
        {
            var child = rt.GetChild(0) as RectTransform;    // 첫 번째 자식 RectTransform 가져오기
            child.anchorMin = Vector2.zero;                 // 자식 앵커 최소값을 (0,0)으로 설정
            child.anchorMax = Vector2.one;                  // 자식 앵커 최대값을 (1,1)로 설정
            child.offsetMin = Vector2.one * borderThickness;   // 안쪽 여백 만큼 offsetMin 설정
            child.offsetMax = Vector2.one * -borderThickness;  // 안쪽 여백 만큼 offsetMax 설정
        }
    }
}

/*
미니맵 크기 자동 조정 스크립트
- RectTransform을 사용하여 크기 조정
- CanvasScaler를 통해 화면 비율에 맞춰 크기 조정
- avoidLabel을 통해 상단 라벨과의 겹침 방지
- marginPixel로 라벨과의 최소 간격 설정
- borderThickness로 안쪽 여백 설정
- OnValidate()를 통해 에디터에서도 즉시 반영
*/
