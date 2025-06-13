using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(RectTransform))]
public class MiniMapToggle_M : MonoBehaviour, IPointerClickHandler
{
    [Header("Refs")]
    [SerializeField] Camera miniCam;        // 미니맵 전용 카메라
    [SerializeField] Camera fullCam;        // 풀맵 전용 카메라
    [SerializeField] RawImage miniRaw;        // 미니맵 RawImage
    [SerializeField] RawImage fullRaw;        // 풀맵 RawImage

    [Header("Map Markers")]
    [SerializeField] Transform[] worldMarkers;    // 목적지 Transform 배열
    [SerializeField] Image iconPrefab;       // 목적지 아이콘(별)

    [Header("Gas Stations")]
    [SerializeField] Transform[] gasStationNodes;
    [SerializeField] Image gasIconPrefab;

    [Header("Animation Settings")]
    [SerializeField] float animTime = 0.25f;
    [SerializeField] float fullCamSize = 75f;
    [SerializeField] float extraTopPadding = 100f;
    [SerializeField] float extraBottomPadding = 100f;
    [SerializeField] float extraLeftPadding = 0f;
    [SerializeField] float extraRightPadding = 0f;

    [Header("UI Panel")]
    [SerializeField] CanvasGroup destPanelCG;

    /* ---------- internal ---------- */
    RectTransform rt;
    Vector2 miniAnchMin, miniAnchMax;
    Vector2 miniSize;
    float miniCamSize;
    bool isFull = false;
    float iconEdgeMargin = 0f;

    class UIIcon { public Image img; public Transform target; }
    List<UIIcon> uiIcons = new();

    void Awake()
    {
        rt = GetComponent<RectTransform>();

        // 저장: 초기(미니) 상태
        miniAnchMin = rt.anchorMin;
        miniAnchMax = rt.anchorMax;
        miniSize = rt.sizeDelta;
        miniCamSize = miniCam.orthographicSize;

        // 초기 카메라/RawImage 상태
        miniCam.enabled = true; miniRaw.enabled = true;
        fullCam.enabled = false; fullRaw.enabled = false;

        if (gasStationNodes == null || gasStationNodes.Length == 0)
        {
            var gos = GameObject.FindGameObjectsWithTag("GasStation");
            Debug.Log($"[MiniMapToggle] found {gos.Length} GasStations");
            gasStationNodes = gos.Select(go => go.transform).ToArray();
        }

        // 1) 목적지 아이콘 생성
        foreach (var m in worldMarkers)
            CreateIcon(m, iconPrefab, m.GetComponent<ParticleSystem>().main.startColor.color);

        var gasObjs = GameObject.FindGameObjectsWithTag("GasStation");
        foreach (var go in gasObjs)
        {
            Image icon = Instantiate(gasIconPrefab, transform.Find("MarkerRoot"));
            icon.transform.SetAsLastSibling();
            // 투명도
            Color c = icon.color;
            c.a = 0.7f;
            icon.color = c;

            uiIcons.Add(new UIIcon { img = icon, target = go.transform });
        }

        SetIconsActive(false);
    }

    //  주유소 초기화 이벤트 구독
    void OnEnable() => DestinationManager.OnGasStationsInitialized += AddGasIcons;
    void OnDisable() => DestinationManager.OnGasStationsInitialized -= AddGasIcons;

    /// DestinationManager 에서 모든 주유소가 배치된 뒤 호출
    void AddGasIcons()
    {
        // 이미 아이콘이 있으면 중복 생성 방지
        if (gasStationNodes != null && gasStationNodes.Length > 0 &&
            uiIcons.Any(u => u.img.sprite == gasIconPrefab.sprite))
            return;

        gasStationNodes = GameObject.FindGameObjectsWithTag("GasStation")
                                    .Select(go => go.transform).ToArray();

        foreach (var g in gasStationNodes)
            CreateIcon(g, gasIconPrefab, Color.white, gasAlpha: 0.6f, scale: 0.9f);   // 색은 필요한 대로

        // 풀맵이 펼쳐져 있다면 즉시 보이도록
        if (isFull) SetIconsActive(true);
    }

    /// <summary>
    /// Transform target에 대해 아이콘 프리팹 인스턴스화 + uiIcons 리스트에 추가
    /// </summary>
    void CreateIcon(Transform target, Image prefab, Color tint, float gasAlpha = 0.7f, float scale = 1.2f)
    {
        var icon = Instantiate(prefab, transform.Find("MarkerRoot"));
        icon.color = tint;
        icon.transform.SetAsLastSibling();
        uiIcons.Add(new UIIcon { img = icon, target = target });

        // 투명도와 색상 적용
        tint.a = gasAlpha;
        icon.color = tint;

        // 크기 조절
        icon.rectTransform.localScale = Vector3.one * scale;
        icon.transform.SetAsLastSibling();
        uiIcons.Add(new UIIcon { img = icon, target = target });
    }

    Coroutine co;

    public void OnPointerClick(PointerEventData e)
    {
        if (co != null) StopCoroutine(co);

        if (isFull)
        {
            // 접힐 때: 풀맵 쓰지 않고 미니맵 카메라/Raw 다시 켬
            fullCam.enabled = false; fullRaw.enabled = false;
            miniCam.enabled = true; miniRaw.enabled = true;
            co = StartCoroutine(Shrink());
        }
        else
        {
            // 펼칠 때: 풀맵 카메라/Raw 켬
            miniCam.enabled = false; miniRaw.enabled = false;
            fullCam.enabled = true; fullRaw.enabled = true;
            co = StartCoroutine(Expand());
        }
        isFull = !isFull;
    }

    void LateUpdate()
    {
        if (!isFull) return;

        var cam = fullCam;  // 풀맵 모드일 때만 아이콘 위치 보정
        float m = iconEdgeMargin;

        foreach (var ui in uiIcons)
        {
            Vector3 vp = cam.WorldToViewportPoint(ui.target.position);
            vp.x = Mathf.Clamp01(vp.x); vp.y = Mathf.Clamp01(vp.y);
            vp.x = Mathf.Lerp(m, 1f - m, vp.x);
            vp.y = Mathf.Lerp(m, 1f - m, vp.y);

            var iconRT = ui.img.rectTransform;
            iconRT.anchorMin = iconRT.anchorMax = new Vector2(vp.x, vp.y);
            iconRT.anchoredPosition = Vector2.zero;
        }
    }

    System.Collections.IEnumerator Expand()
    {
        SetIconsActive(true);

        var cs = GetComponentInParent<CanvasScaler>();
        float refW = cs.referenceResolution.x;
        float refH = cs.referenceResolution.y;

        Vector2 targetMin = new Vector2(extraLeftPadding / refW,
                                        extraBottomPadding / refH);
        Vector2 targetMax = new Vector2(1f - extraRightPadding / refW,
                                        1f - extraTopPadding / refH);

        Vector2 startMin = rt.anchorMin, startMax = rt.anchorMax;
        float camStart = fullCam.orthographicSize;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / animTime;
            float k = Mathf.SmoothStep(0f, 1f, t);

            rt.anchorMin = Vector2.Lerp(startMin, targetMin, k);
            rt.anchorMax = Vector2.Lerp(startMax, targetMax, k);
            fullCam.orthographicSize = Mathf.Lerp(camStart, fullCamSize, k);

            yield return null;
        }
    }

    System.Collections.IEnumerator Shrink()
    {
        SetIconsActive(false);

        float t = 0f;
        Vector2 startMin = rt.anchorMin, startMax = rt.anchorMax;
        float camStart = miniCam.orthographicSize;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / animTime;
            float k = Mathf.SmoothStep(0f, 1f, t);

            rt.anchorMin = Vector2.Lerp(startMin, miniAnchMin, k);
            rt.anchorMax = Vector2.Lerp(startMax, miniAnchMax, k);
            miniCam.orthographicSize = Mathf.Lerp(camStart, miniCamSize, k);

            yield return null;
        }
    }

    void SetIconsActive(bool on)
    {
        foreach (var ui in uiIcons)
            ui.img.enabled = on;
    }
}
