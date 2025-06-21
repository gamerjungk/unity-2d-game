using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(RectTransform))]
public class MiniMapToggle_M : MonoBehaviour, IPointerClickHandler
{
    [Header("Refs")] // 헤더 표시
    [SerializeField] Camera miniCam; // 미니맵 전용 카메라
    [SerializeField] Camera fullCam; // 풀맵 전용 카메라
    [SerializeField] RawImage miniRaw; // 미니맵 RawImage
    [SerializeField] RawImage fullRaw; // 풀맵 RawImage

    [Header("Map Markers")] // 헤더 표시
    [SerializeField] Transform[] worldMarkers; // 목적지 Transform 배열
    [SerializeField] Image iconPrefab; // 목적지 아이콘(별)

    [Header("Gas Stations")] // 헤더 표시
    [SerializeField] Transform[] gasStationNodes; // 주유소 Transform 배열
    [SerializeField] Image gasIconPrefab; // 주유소 아이콘(배터리)

    [Header("Animation Settings")] // 헤더 표시
    [SerializeField] float animTime = 0.25f; // 미니맵 펼치기/접기 애니메이션 시간
    [SerializeField] float fullCamSize = 75f; // 풀맵 모드 카메라 orthographicSize 목표값
    [SerializeField] float extraTopPadding = 100f; // 풀맵 모드 상단 여유 패딩
    [SerializeField] float extraBottomPadding = 100f; // 풀맵 모드 하단 여유 패딩
    [SerializeField] float extraLeftPadding = 0f; // 풀맵 모드 좌측 여유 패딩
    [SerializeField] float extraRightPadding = 0f; // 풀맵 모드 우측 여유 패딩

    [Header("UI Panel")] // 헤더 표시
    [SerializeField] CanvasGroup destPanelCG; // 투명도 조절용 목적지 패널

    /* ---------- internal ---------- */
    RectTransform rt; // RectTransform 캐시
    Vector2 miniAnchMin, miniAnchMax; // 미니맵 모드일 때 앵커 최소/최대 값 저장
    Vector2 miniSize; // 미니맵 모드 크기 저장
    float miniCamSize; // 미니맵 모드 카메라 orthographicSize 저장
    bool isFull = false; // 현재 풀맵 모드 여부 플래그
    float iconEdgeMargin = 0f; // 아이콘 뷰포트 가장자리 마진

    class UIIcon { public Image img; public Transform target; } // 아이콘과 타깃 Transform을 묶는 내부 클래스
    List<UIIcon> uiIcons = new(); // 생성된 UIIcon 목록

    void Awake()
    {
        rt = GetComponent<RectTransform>(); // RectTransform 컴포넌트 가져오기

        // 초기(미니) 앵커와 크기, 카메라 설정 상태
        miniAnchMin = rt.anchorMin;
        miniAnchMax = rt.anchorMax;
        miniSize = rt.sizeDelta;
        miniCamSize = miniCam.orthographicSize;

        // 미니맵 모드 활성화, 풀맵 모드 비활성화
        miniCam.enabled = true; miniRaw.enabled = true;
        fullCam.enabled = false; fullRaw.enabled = false;

        // 주유소 노드 배열이 없으면 태그로 찾아 할당
        if (gasStationNodes == null || gasStationNodes.Length == 0)
        {
            // "GasStation" 태그가 붙은 모든 게임오브젝트를 찾아 배열로 저장
            var gos = GameObject.FindGameObjectsWithTag("GasStation");
            // 찾은 개수를 로그로 출력
            Debug.Log($"[MiniMapToggle] found {gos.Length} GasStations");
            // Transform 배열만 추출해 gasStationNodes에 저장
            gasStationNodes = gos.Select(go => go.transform).ToArray();
        }

        // 1) 목적지 아이콘 생성
        foreach (var m in worldMarkers)
            // 파티클의 시작 색상을 가져와 tint로 사용
            CreateIcon(m, iconPrefab, m.GetComponent<ParticleSystem>().main.startColor.color);

        // 2) 기존 씬에 있는 주유소 객체에도 아이콘 생성
        var gasObjs = GameObject.FindGameObjectsWithTag("GasStation");
        foreach (var go in gasObjs)
        {
            // MarkerRoot 자식으로 gasIconPrefab 인스턴스화
            Image icon = Instantiate(gasIconPrefab, transform.Find("MarkerRoot"));
            // 렌더 순서를 최상위로 올려 다른 UI 위에 표시
            icon.transform.SetAsLastSibling();
            // 아이콘의 현재 색상 가져오기
            Color c = icon.color;
            // 알파(투명도)만 0.7f로 설정
            c.a = 0.7f;
            // 수정된 색상을 아이콘에 적용
            icon.color = c;

            // 생성된 아이콘과 해당 타깃 Transform을 리스트에 추가
            uiIcons.Add(new UIIcon { img = icon, target = go.transform });
        }

        // 모든 아이콘 비활성화해서 시작 상태에선 보이지 않게 설정
        SetIconsActive(false);
    }

    //  주유소 초기화 이벤트 구독
    void OnEnable() => DestinationManager.OnGasStationsInitialized += AddGasIcons;
    void OnDisable() => DestinationManager.OnGasStationsInitialized -= AddGasIcons;

    // DestinationManager 에서 모든 주유소가 배치된 뒤 호출
    void AddGasIcons()
    {
        // 이미 아이콘이 있으면 중복 생성 방지
        if (gasStationNodes != null && gasStationNodes.Length > 0 &&
            uiIcons.Any(u => u.img.sprite == gasIconPrefab.sprite))
            return;

        // 씬에서 "GasStation" 태그가 붙은 모든 게임 오브젝트를 찾아 Transform 배열로 저장
        gasStationNodes = GameObject.FindGameObjectsWithTag("GasStation")
                                    .Select(go => go.transform).ToArray();

        // 각 주유소 Transform에 대해 아이콘을 생성하고 색상, 투명도, 크기 설정
        foreach (var g in gasStationNodes)
            CreateIcon(g, gasIconPrefab, Color.white, gasAlpha: 0.6f, scale: 0.9f);   // 색은 필요한 대로

        // 풀맵이 펼쳐져 있다면 즉시 보이도록 표시
        if (isFull) SetIconsActive(true);
    }

    // Transform target에 대해 아이콘 프리팹 인스턴스화 + uiIcons 리스트에 추가
    void CreateIcon(Transform target, Image prefab, Color tint, float gasAlpha = 0.7f, float scale = 1.2f)
    {
        // MarkerRoot 자식으로 아이콘 프리팹을 인스턴스화하고 Image 컴포넌트 참조를 반환
        var icon = Instantiate(prefab, transform.Find("MarkerRoot"));
        // 기본 색상 = tint 값
        icon.color = tint;
        // 렌더 순서를 최상위로 올려 다른 UI 위에 표시되도록 설정
        icon.transform.SetAsLastSibling();
        // 생성된 아이콘과 그 타깃 Transform을 uiIcons 리스트에 추가
        uiIcons.Add(new UIIcon { img = icon, target = target });

        // 투명도와 색상 적용
        tint.a = gasAlpha;
        icon.color = tint;

        // 크기 조절
        icon.rectTransform.localScale = Vector3.one * scale; //이아콘 스케일 = Vector3.one * scale 배율
        icon.transform.SetAsLastSibling(); // 렌더 순서 최상위로 설정
        uiIcons.Add(new UIIcon { img = icon, target = target }); // usIcons 리스트에 중복 추가
    }

    Coroutine co;

    // 클릭 이벤트 처리 함수
    public void OnPointerClick(PointerEventData e)
    {
        if (co != null) StopCoroutine(co); // 실행 중인 코루틴이 있으면 해당 애니메이션 코루틴 중단

        if (isFull) // 풀맵 모드이면
        {
            // 접힐 때: 풀맵 쓰지 않고 미니맵 카메라/Raw 다시 켬
            fullCam.enabled = false; // 풀맵 카메라 끔
            fullRaw.enabled = false; // 풀맵 RawImage 끔
            miniCam.enabled = true; // 미니맵 카메라 켬
            miniRaw.enabled = true; // 미니맵 RawImage 켬
            co = StartCoroutine(Shrink()); // 축소 애니메이션 코루틴 시작
        }
        else // 미니맵 모드이면
        {
            // 펼칠 때: 풀맵 카메라/Raw 켬
            fullCam.enabled = false; // 풀맵 카메라 끔
            fullRaw.enabled = false; // 풀맵 RawImage 끔
            miniCam.enabled = true; // 미니맵 카메라 켬
            miniRaw.enabled = true; // 미니맵 RawImage 켬
            co = StartCoroutine(Shrink()); // 축소 애니메이션 코루틴 시작
        }
        isFull = !isFull; // 모드 상태 토글: 풀맵 <-> 미니맵
    }

    void LateUpdate()
    {
        if (!isFull) return; // 풀맵 모드 아니면 함수 종료

        var cam = fullCam;  // 풀맵 모드일 때만 아이콘 위치 보정
        float m = iconEdgeMargin; // 아이콘 뷰포트 가장자리 마진 값 저장

        // 등록된 모든 아이콘에 대해 반복
        foreach (var ui in uiIcons)
        {
            // 월드 좌표의 타깃 위치를 뷰포트 좌표(0~1)로 변환  
            Vector3 vp = cam.WorldToViewportPoint(ui.target.position);
            vp.x = Mathf.Clamp01(vp.x); // X값을 0~1 범위로 고정
            vp.y = Mathf.Clamp01(vp.y); // y값을 0~1 범위로 고정

            // 마진을 적용하여 뷰포트 내에서 아이콘이 화면 밖으로 벗어나지 않게 조정
            vp.x = Mathf.Lerp(m, 1f - m, vp.x);
            vp.y = Mathf.Lerp(m, 1f - m, vp.y);

            var iconRT = ui.img.rectTransform; // 아이콘의 RectTransform 가져오기

            // 앵커를 뷰포트 좌표 위치로 설정(아이콘 위치 고정)
            iconRT.anchorMin = iconRT.anchorMax = new Vector2(vp.x, vp.y);
            iconRT.anchoredPosition = Vector2.zero; // 앵커 기준 로컬 위치를 (0,0)으로 설정
        }
    }

    // 미니맵을 풀맵으로 확장하는 애니메이션 처리하는 함수
    System.Collections.IEnumerator Expand()
    {
        SetIconsActive(true); // 모든 아이콘 활성화(보임 처리)

        var cs = GetComponentInParent<CanvasScaler>(); // 부모 CanvasScaler 컴포넌트 가져오기
        float refW = cs.referenceResolution.x; // 캔버스 기준 가로 해상도 저장
        float refH = cs.referenceResolution.y; // 캔버스 기준 세로 해상도 저장

        // 풀맵 모드에서의 앵커 최소값(왼쪽 하단)을 화면 비율로 계산
        Vector2 targetMin = new Vector2(extraLeftPadding / refW, // 좌측 패딩 비율
                                        extraBottomPadding / refH); // 히딘 패딩 비율
         // 풀맵 모드에서의 앵커 최대값(오른쪽 상단)을 화면 비율로 계산
        Vector2 targetMax = new Vector2(1f - extraRightPadding / refW, // 우측 패딩 비율을 뺀 값
                                        1f - extraTopPadding / refH); // 상단 패딩 비율을 뺀 값

        Vector2 startMin = rt.anchorMin, startMax = rt.anchorMax; // 현재 앵커(min, max)값 저장
        float camStart = fullCam.orthographicSize; // 풀맵 카메라 시작 orthographicSize 저장
        float t = 0f; // 보간 파라미터 초기화

        // t가 1보다 작을 동안 반복
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / animTime; // 언스케일 타임으로 t 증가 (animTime 동안 0→1)
            float k = Mathf.SmoothStep(0f, 1f, t); // t를 부드럽게 보간한 계수 k 계산

            // 앵커를 시작 값에서 목표 값으로 보간
            rt.anchorMin = Vector2.Lerp(startMin, targetMin, k);
            rt.anchorMax = Vector2.Lerp(startMax, targetMax, k);
            // 카메라 orthographicSize를 시작 값에서 목표 값으로 보간
            fullCam.orthographicSize = Mathf.Lerp(camStart, fullCamSize, k);

            yield return null; // 다음 프레임까지 대기
        }
    }

    // 미니맵으로 축소하는 애니메이션 처리하는 함수
    System.Collections.IEnumerator Shrink()
    {
        SetIconsActive(false); // 모든 아이콘 비활성화(숨김 처리)

        float t = 0f; // 애니메이션 진행시간 비율(0→1) 초기화
        Vector2 startMin = rt.anchorMin, startMax = rt.anchorMax; // 현재 RectTransform의 앵커(min, max) 값 저장
        float camStart = miniCam.orthographicSize; // 미니맵 카메라의 현재 orthographicSize 저장

        // t가 1보다 작으면 계속 반복
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / animTime; // 실제 경과 시간에 비례해 t 증가 (언스케일 타임 사용)
            float k = Mathf.SmoothStep(0f, 1f, t); // t를 부드럽게 보간한 계수 k 계산

            rt.anchorMin = Vector2.Lerp(startMin, miniAnchMin, k); // anchorMin을 원래 값에서 미니맵 값으로 보간
            rt.anchorMax = Vector2.Lerp(startMax, miniAnchMax, k); // anchorMax을 원래 값에서 미니맵 값으로 보간
            miniCam.orthographicSize = Mathf.Lerp(camStart, miniCamSize, k); // 카메라 크기를 보간하여 축소

            yield return null; // 다음 프레임까지 대기 후 루프 재개
        }
    }

    // 모든 UIIcon 이미지 활성화 or 비활성화
    void SetIconsActive(bool on)
    {

        // uiIcons 리스트에 저장된 각 UIIcon에 대해 반복
        foreach (var ui in uiIcons)
            ui.img.enabled = on; // Image 컴포넌트의 enabled 속성을 on 값으로 설정
    }
}

/*
    미니맵 토글 스크립트
    - 미니맵과 풀맵을 전환하는 기능
    - 목적지와 주유소 아이콘을 표시하도록 함
    - 풀맵 모드에서 아이콘 위치를 화면 가장자리 여백에 맞춰 조정
    
    - 미니맵 카메라와 풀맵 카메라를 각각 할당
    - RawImage 컴포넌트를 통해 미니맵과 풀맵을 표시
    - 목적지와 주유소 Transform 배열을 설정하여 아이콘 생성
*/