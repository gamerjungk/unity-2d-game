using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class MiniMapToggle_M : MonoBehaviour, IPointerClickHandler
{
    [Header("Refs")]
    [SerializeField] Camera miniCam;                 // 미니맵 전용 카메라
    [SerializeField] Camera fullCam;             // FullMapCamera
    [SerializeField] RawImage miniRaw;           // MiniMap Raw-Image
    [SerializeField] RawImage fullRaw;           // FullMap Raw-Image
    [SerializeField] Transform[] worldMarkers;       // DestinationManager.Markers 넣기
    [SerializeField] Image iconPrefab;               // ★ 또는 ● 프리팹
    [SerializeField] float animTime = 0.25f;        // 패널 확대/축소 속도
    [SerializeField] float fullPadding = 80f;        // 전체맵 모서리 여백(px)
    [SerializeField] float fullCamSize = 75f;       // 도시 전체가 보이도록 orthographicSize
    [SerializeField] CanvasGroup destPanelCG;
    [SerializeField] float extraTopPadding = 100;      // 상단 UI 덮지 않도록
    [SerializeField] float extraBottomPadding = 100;  // 하단 회색바 여백
    [SerializeField] float extraLeftPadding = 0;   // 왼쪽 여백 (px)
    [SerializeField] float extraRightPadding = 0;   // 오른쪽 여백 (px)


    /* ---------- 내부 ---------- */
    RectTransform rt;
    Vector2 miniSize;            // 작은 상태 사이즈
    Vector2 miniAnchMin, miniAnchMax;
    float miniCamSize;          // 원래 ortho Size
    bool isFull = false;
    float iconEdgeMargin = 0.0f;

    class UIIcon
    {
        public Image img;
        public Transform target;      // 월드 목적지 Transform
    }
    List<UIIcon> uiIcons = new();

    void Awake()
    {
        rt = GetComponent<RectTransform>();

        // 작은 상태(초기) 값 저장
        miniAnchMin = rt.anchorMin;
        miniAnchMax = rt.anchorMax;
        miniSize = rt.sizeDelta;
        miniCamSize = miniCam.orthographicSize;

        miniCam.enabled = true;   // ← 처음엔 ‘작은’ 카메라만 켜기
        miniRaw.enabled = true;

        fullCam.enabled = false;  // ‘풀맵’은 꺼 두기
        fullRaw.enabled = false;

        /* ---------- 목적지 아이콘 동적 생성 ---------- */
        foreach (var m in worldMarkers)
        {
            Image icon = Instantiate(iconPrefab, transform.Find("MarkerRoot"));
            icon.color = m.GetComponent<ParticleSystem>().main.startColor.color;
            icon.transform.SetAsLastSibling();
            uiIcons.Add(new UIIcon { img = icon, target = m });
        }
        SetIconsActive(false);
    }

    Coroutine co;

    /* ======== 토글 클릭 ======== */
    public void OnPointerClick(PointerEventData e)
    {
        if (co != null) StopCoroutine(co);

        if (isFull)   // ▶ 접힐 때
        {
            fullCam.enabled = false;
            fullRaw.enabled = false;
            miniCam.enabled = true;
            miniRaw.enabled = true;

            co = StartCoroutine(Shrink());
        }
        else          // ▶ 펼칠 때
        {
            miniCam.enabled = false;
            miniRaw.enabled = false;
            fullCam.enabled = true;
            fullRaw.enabled = true;

            co = StartCoroutine(Expand());
        }
        isFull = !isFull;
    }


    /* ======== 매 프레임 아이콘 위치 보정 ======== */
    void LateUpdate()
    {
        if (!isFull) return;

        Camera Cam = isFull ? fullCam : miniCam;
        float m = iconEdgeMargin;        // 가독성을 위해 변수에 저장

        foreach (var ui in uiIcons)
        {
            Vector3 vp = Cam.WorldToViewportPoint(ui.target.position);

            /* ▼ ① Viewport 값을 0~1 → m~1-m 로 잘라내기(Clamp) */
            vp.x = Mathf.Clamp01(vp.x);
            vp.y = Mathf.Clamp01(vp.y);
            vp.x = Mathf.Lerp(m, 1f - m, vp.x);    // m-(1-m) 범위로 다시 매핑
            vp.y = Mathf.Lerp(m, 1f - m, vp.y);

            /* ▼ ② 이미지의 Anchor 를 바로 Viewport 로 */
            RectTransform r = ui.img.rectTransform;
            r.anchorMin = r.anchorMax = new Vector2(vp.x, vp.y);
            r.anchoredPosition = Vector2.zero;     // 안 쓰므로 0
        }
    }


    /* =================================================================== */
    /*                                코 루 튠                             */
    /* =================================================================== */

    System.Collections.IEnumerator Expand()
    {
        SetIconsActive(true);

        CanvasScaler cs = GetComponentInParent<CanvasScaler>();
        float refW = cs.referenceResolution.x;   // 예: 1080
        float refH = cs.referenceResolution.y;   // 예: 1920

        /* ─ 새 좌표 (여백 포함) ─ */
        Vector2 targetMin = new(
            extraLeftPadding / refW,      // ← 왼쪽 패딩
            extraBottomPadding / refH);     // ↓ 아래 패딩

        Vector2 targetMax = new(
            1 - extraRightPadding / refW,   // → 오른쪽 패딩
            1 - extraTopPadding / refH); // ↑ 위 패딩

        /* ─ 애니메이션 준비 ─ */
        Vector2 startMin = rt.anchorMin;
        Vector2 startMax = rt.anchorMax;
        float camStart = fullCam.orthographicSize;
        float t = 0;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / animTime;
            float k = Mathf.SmoothStep(0, 1, t);

            /* ─ 여기!  targetMin / targetMax 로 보간 ─ */
            rt.anchorMin = Vector2.Lerp(startMin, targetMin, k);
            rt.anchorMax = Vector2.Lerp(startMax, targetMax, k);

            fullCam.orthographicSize = Mathf.Lerp(camStart, fullCamSize, k);
            yield return null;
        }
    }


    System.Collections.IEnumerator Shrink()
    {
        SetIconsActive(false);

        float t = 0;
        Vector2 startMin = rt.anchorMin;
        Vector2 startMax = rt.anchorMax;
        float camStart = miniCam.orthographicSize;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / animTime;
            float k = Mathf.SmoothStep(0, 1, t);

            rt.anchorMin = Vector2.Lerp(startMin, miniAnchMin, k);
            rt.anchorMax = Vector2.Lerp(startMax, miniAnchMax, k);
            miniCam.orthographicSize = Mathf.Lerp(camStart, miniCamSize, k);
            yield return null;
        }
    }

    /* ---------- 헬퍼 ---------- */
    void SetIconsActive(bool on)
    {
        foreach (var ui in uiIcons) ui.img.enabled = on;
    }
}
