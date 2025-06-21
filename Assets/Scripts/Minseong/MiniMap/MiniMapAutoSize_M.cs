using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class MiniMapAutoSize_M : MonoBehaviour
{
    [Header("비율 설정")]
    [Tooltip("가로 화면 기준 % (ex) 0.25 => 화면 폭의 25 %")]
    [Range(0.05f, 0.5f)] public float widthPercent = 0.25f;
    [Tooltip("세로 화면 기준 % (ex) 0.14 => 화면 높이의 14 %")]
    [Range(0.05f, 0.5f)] public float heightPercent = 0.14f;

    [Header("겹침 방지 대상")]
    [Tooltip("cur / Turn 등의 상단 라벨 RectTransform (없으면 무시)")]
    public RectTransform avoidLabel;

    [Header("여유 간격")]
    public float marginPixel = 8f;          // 라벨과의 최소 간격

    public float borderThickness = 8f;

    RectTransform rt;
    CanvasScaler scaler;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        ApplySize();
    }

#if UNITY_EDITOR        // 에디터에서도 즉시 반영
    void OnValidate()
    {
        if (!rt) rt = GetComponent<RectTransform>();
        if (!scaler) scaler = GetComponentInParent<CanvasScaler>();
        ApplySize();
    }
#endif

    void ApplySize()
    {
        /* ────────────────────────────── 1. 기본 크기 ──────────────────────────── */
        Vector2 refRes = scaler                     // CanvasScaler 가 있으면
                         ? scaler.referenceResolution
                         : new Vector2(Screen.width, Screen.height); // (폴백)

        float targetW = refRes.x * widthPercent;   // “% of 1080” 같은 식
        float targetH = refRes.y * heightPercent;

        /* ────────────────────────────── 2. 라벨 피하기 ───────────────────────── */
        if (avoidLabel)
        {
            var wc = new Vector3[4];
            avoidLabel.GetWorldCorners(wc);                        // 라벨 4 점
            float labelBottom = RectTransformUtility
                                .WorldToScreenPoint(null, wc[0]).y;

            float mapTop = targetH + marginPixel;                  // 맵 상단
            if (mapTop > labelBottom)
            {
                float offset = mapTop - labelBottom;
                rt.anchoredPosition -= new Vector2(0, offset);     // ↓ 내리기
            }
        }

        /* ────────────────────────────── 3. 사이즈 / 보더 ────────────────────── */
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetW);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetH);

        if (rt.childCount > 0)                         // 안쪽 Raw-Image 조절
        {
            var child = rt.GetChild(0) as RectTransform;
            child.anchorMin = Vector2.zero;
            child.anchorMax = Vector2.one;
            child.offsetMin = Vector2.one * borderThickness;      // + 안쪽
            child.offsetMax = Vector2.one * -borderThickness;      // – 안쪽
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
