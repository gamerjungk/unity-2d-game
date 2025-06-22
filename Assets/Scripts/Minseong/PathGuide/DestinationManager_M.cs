using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using Random = UnityEngine.Random;

// 목적지 관리 시스템 처리 매니저
public class DestinationManager : MonoBehaviour
{
    /* ────────── 싱글턴 ────────── */
    public static DestinationManager Instance { get; private set; } // 싱글턴 인스턴스 프로퍼티
    public static event Action OnGasStationsInitialized; // 주유소 초기화 완료 이벤트
    public static event Action<int> OnArrivedTarget; // UI 도착 알리는 이벤트

    /* ────────── 외부 연결 ────────── */
    [Header("External refs")] // 인스펙터 구분용 헤더 표시
    [SerializeField] Transform player; // 플레이어 Transform 참조
    [SerializeField] NavMeshSurface surface; // RoadRoot(NavMeshSurface)
    [Tooltip("Hierarchy 에 있는 4개의 Destination_* 파티클")]
    [SerializeField] Transform[] markers;         // 목적지 마커(0~3)


    [Header("Option")] // 옵션 헤더 표시
    [Tooltip("목적지끼리 최소 거리(m)")]
    [Range(1, 50)] public float minDistanceBetween = 12f; // 마커 사이 최소 거리 설정
    private Vector3? lastTargetPosition = null; // 이전 목표 위치 저장용

    /* ────────── 런타임 상태 ────────── */
    readonly List<Transform> roadNodes = new();   // 현재 활성 도로노드 목룍
    public List<Transform> stations = new(); // 주유소 위치 목록
    public Transform CurrentTarget { get; private set; } // 현재 선택된 목표 Transform

    private bool isPickupPhase = true; // true면 다음 도착은 픽업, false면 배달


    /* ===================================================================== */
    #region Unity Lifecycle
    /* ===================================================================== */

    void Awake()
    {
        if (Instance == null) Instance = this; // 싱글턴 인스턴스 설정
        else { Destroy(gameObject); return; } // 이미 있으면 중복 파괴

        // 마커 배열 미설정 or 갯수 오류 일시
        if (markers == null || markers.Length != 4)
        {
            Debug.LogError("DestinationManager ▸ markers 배열이 비어 있거나 4개가 아닙니다.");
            enabled = false; // 스크립트 비활성화
            return;
        }
    }

    // NavMesh 빌드 & RoadGenerator 등이 끝난 다음-프레임에 초기화
    IEnumerator Start()
    {
        yield return null; // 한 프레임 대기

        RefreshRoadNodeList(); // 도로 노드 목록 갱신
        SetPlace(ref stations, "GasStation", 7); // 주유소 7개 배치
        PlaceAllMarkersRandom(); // 마커 랜덤 배치
        SelectTarget(0); // 첫 번째 마커 선택
        lastTargetPosition = player.position; // 시작 위치 저장
    }

    #endregion  
    /* ===================================================================== */

    /* ===================================================================== */
    #region Public API (다른 스크립트/UI에서 호출)
    /* ===================================================================== */

    // UI 버튼에서 호출 (idx = 0~3)
    public void SelectTarget(int idx)
    {
        // 배달 단계일 때 목표 변경 불가
        if (!isPickupPhase)
        {
            Debug.LogWarning($"[{nameof(SelectTarget)}] 배달 단계에는 목표를 변경할 수 없습니다.");
            return;
        }

        // 인덱스 범위 체크
        if (idx < 0 || idx >= markers.Length) return;

        // 현재 목표 설정
        CurrentTarget = markers[idx];
        // 경로 시각화 요청
        PathDrawer_m.Instance?.DrawPath(player, CurrentTarget);
    }

    // 각 마커의 픽업 위치 저장 배열
    private Vector3?[] pickupPositions = new Vector3?[4];

    // 플레이어가 현재 타깃에 도달했을 때 MoneyTrigger → PlayerPath → 여기
    public void ArrivedCurrentTarget()
    {
        // 목표 설정 없으면 무시
        if (CurrentTarget == null) return;

        // 현재 목표 인덱스 찾기
        int idx = Array.IndexOf(markers, CurrentTarget);
        if (idx < 0) return;

        // 도착 위치
        Vector3 currentPos = CurrentTarget.position;

        // UI 상태 조회
        bool isPickup = DestinationUI_M.Instance.GetPickupState(idx);

        // 도착 이벤트 알림
        OnArrivedTarget?.Invoke(idx);

        // 이전 위치가 있다면
        if (isPickup)
        {
            // 유클리드 거리 계산
            float dx = currentPos.x - lastTargetPosition.Value.x;
            float dy = currentPos.y - lastTargetPosition.Value.y;
            float dz = currentPos.z - lastTargetPosition.Value.z;
            float distance = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);

            int reward = Mathf.RoundToInt(distance * 100); // 보상 계산
            GameDataManager.Instance.AddMoney(reward); // 보상 지급

            Debug.Log($"도착한 마커 거리: {distance:F2}m → 보상 {reward} 지급");
        }
        else
        {
            Debug.Log("🚩 최초 도착: 보상 없음 (거리 기준 없음)");
        }
        
        // 픽업 위치 초기화
        pickupPositions[idx] = null;

        // 다음 목적지 배치
        MoveMarkerRandom(CurrentTarget);
        PathDrawer_m.Instance?.DrawPath(player, CurrentTarget);
    }

    // 실시간 도로 On/Off 후 호출 (RoadToggle.cs)
    public void RebuildNavMesh() => surface.BuildNavMesh();

    public Transform[] Markers => markers; // 마커 배열 노출
    public Transform Player => player; // 플레이어 Transform 노출

    #endregion
    /* ===================================================================== */

    /* ===================================================================== */
    #region Marker 배치 · 이동
    /* ===================================================================== */

    void RefreshRoadNodeList()
    {
        roadNodes.Clear(); // 기존 목록 비우기

        foreach (var go in GameObject.FindGameObjectsWithTag("RoadNode"))
        {
            if (!go.activeInHierarchy) continue; // 비활성화 노드 무시

            // 해당 노드가 NavMesh 에 실제로 포함돼 있는지 확인
            if (NavMesh.SamplePosition(go.transform.position, out _, 0.25f, NavMesh.AllAreas))
                roadNodes.Add(go.transform);
        }
    }

    void PlaceAllMarkersRandom()
    {
        // 모든 마커에 대해 랜덤 위치로 이동
        foreach (var m in markers)
            MoveMarkerRandom(m);

        surface.BuildNavMesh(); // 최초 1회 베이크
    }

    void MoveMarkerRandom(Transform marker)
    {
        RefreshRoadNodeList(); // 항상 최신 노드 목록 사용
        if (roadNodes.Count == 0) return;

        const int maxTry = 100; // 최대 시도 횟수
        for (int t = 0; t < maxTry; ++t)
        {
            Transform node = roadNodes[Random.Range(0, roadNodes.Count)];
            if (IsTooClose(node.position, marker)) continue; // 너무 가까우면 재시도

            marker.position = node.position + Vector3.up * 0.3f; // 마커 높이 보정
            marker.GetComponent<MoneyTrigger>()?.ResetTrigger(); // 충돌 플래그 초기화
            return;
        }
        Debug.LogWarning("MoveMarkerRandom ▸ 조건에 맞는 RoadNode 를 찾지 못했습니다.");
    }

    bool IsTooClose(Vector3 pos, Transform self)
    {
        foreach (var m in markers)
        {
            if (m == self) continue;
            if (Vector3.Distance(pos, m.position) < minDistanceBetween) return true;
        }
        return false;
    }

    void SetPlace(ref List<Transform> place, string tag, int count)
    {
        place.Clear(); // 기존 목록 비우기
        for (int i = 0; i < count; i++)
        {
            Transform node = roadNodes[Random.Range(0, roadNodes.Count)];
            roadNodes.Remove(node); // 중복 방지 위해 제거
            node.tag = tag; // 태그 설정
            place.Add(node); // 목록에 추가
            GameManager.inst.pool.Spawn(tag, node.position, Quaternion.Euler(90, 0, 0)); // 객체 생성
        }

        // 주유소 초기화 이벤트 발행
        OnGasStationsInitialized?.Invoke();
    }

    #endregion
    /* ===================================================================== */
}
/*
도로 노드와 목적지 마커를 관리하는 스크립트

    - 싱글턴 패턴으로 구현되어 게임 전역에서 접근 가능
    - 플레이어의 현재 목적지 관리 및 도달 여부 확인
    - 목적지 마커를 도로 노드에 랜덤하게 배치
    - 주유소와 같은 특정 장소를 초기화하고 관리
    - UI 이벤트와 연동하여 목적지 선택 및 도달 알림
    
    + PlayerPath_M.cs 스크립트와 함께 사용되어 플레이어가 목적지에 도달했는지 확인하는 기능을 수행
*/