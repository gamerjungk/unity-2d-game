using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using Random = UnityEngine.Random;

public class DestinationManager : MonoBehaviour
{
    /* ────────── 싱글턴 ────────── */
    public static DestinationManager Instance { get; private set; }
    public static event Action OnGasStationsInitialized;

    /* ────────── 외부 연결 ────────── */
    [Header("External refs")]
    [SerializeField] Transform player;
    [SerializeField] NavMeshSurface surface;      // RoadRoot(NavMeshSurface)
    [Tooltip("Hierarchy 에 있는 4개의 Destination_* 파티클")]
    [SerializeField] Transform[] markers;         // 0~3


    [Header("Option")]
    [Tooltip("목적지끼리 최소 거리(m)")]
    [Range(1, 50)] public float minDistanceBetween = 12f;
    private Vector3? lastTargetPosition = null;

    /* ────────── 런타임 상태 ────────── */
    readonly List<Transform> roadNodes = new();   // 현재 활성 도로노드
    public List<Transform> stations = new(); // 주유소
    public Transform CurrentTarget { get; private set; }

    /* ===================================================================== */
    #region Unity Lifecycle
    /* ===================================================================== */

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (markers == null || markers.Length != 4)
        {
            Debug.LogError("DestinationManager ▸ markers 배열이 비어 있거나 4개가 아닙니다.");
            enabled = false;
            return;
        }
    }

    // NavMesh 빌드 & RoadGenerator 등이 끝난 다음-프레임에 초기화
    IEnumerator Start()
    {
        yield return null;                 // 한 프레임 대기

        RefreshRoadNodeList();
        SetPlace(ref stations, "GasStation", 7);
        PlaceAllMarkersRandom();
        SelectTarget(0);                   // 기본 목표
        lastTargetPosition = player.position;
    }

    #endregion  
    /* ===================================================================== */

    /* ===================================================================== */
    #region Public API (다른 스크립트/UI에서 호출)
    /* ===================================================================== */

    /// UI 버튼에서 호출 (idx = 0~3)
    public void SelectTarget(int idx)
    {
        if (idx < 0 || idx >= markers.Length) return;

        CurrentTarget = markers[idx];
        PathDrawer_m.Instance?.DrawPath(player, CurrentTarget);
    }

    /// 플레이어가 현재 타깃에 도달했을 때 MoneyTrigger → PlayerPath → 여기
    public void ArrivedCurrentTarget()
    {
        if (CurrentTarget == null) return;

        Vector3 currentPos = CurrentTarget.position;

        if (lastTargetPosition.HasValue)
        {
            // 유클리드 거리 계산
            float dx = currentPos.x - lastTargetPosition.Value.x;
            float dy = currentPos.y - lastTargetPosition.Value.y;
            float dz = currentPos.z - lastTargetPosition.Value.z;
            float distance = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);

            int reward = Mathf.RoundToInt(distance * 100);
            GameDataManager.Instance.AddMoney(reward);

            Debug.Log($"도착한 마커 거리: {distance:F2}m → 보상 {reward} 지급");
        }
        else
        {
            Debug.Log("🚩 최초 도착: 보상 없음 (거리 기준 없음)");
        }

        // 이번 마커 위치를 다음 비교 기준으로 저장
        lastTargetPosition = currentPos;

        // 마커 이동
        MoveMarkerRandom(CurrentTarget);

        // 경로 그리기
        PathDrawer_m.Instance?.DrawPath(player, CurrentTarget);
    }

    /// 실시간 도로 On/Off 후 호출 (RoadToggle.cs)
    public void RebuildNavMesh() => surface.BuildNavMesh();

    public Transform[] Markers => markers;
    public Transform Player => player;

    #endregion
    /* ===================================================================== */

    /* ===================================================================== */
    #region Marker 배치 · 이동
    /* ===================================================================== */

    void RefreshRoadNodeList()
    {
        roadNodes.Clear();

        foreach (var go in GameObject.FindGameObjectsWithTag("RoadNode"))
        {
            if (!go.activeInHierarchy) continue;

            // 해당 노드가 NavMesh 에 실제로 포함돼 있는지
            if (NavMesh.SamplePosition(go.transform.position, out _, 0.25f, NavMesh.AllAreas))
                roadNodes.Add(go.transform);
        }
    }

    void PlaceAllMarkersRandom()
    {
        foreach (var m in markers)
            MoveMarkerRandom(m);

        surface.BuildNavMesh();            // 최초 1회 베이크
    }

    void MoveMarkerRandom(Transform marker)
    {
        RefreshRoadNodeList();             // 항상 최신 노드 목록 사용
        if (roadNodes.Count == 0) return;

        const int maxTry = 100;
        for (int t = 0; t < maxTry; ++t)
        {
            Transform node = roadNodes[Random.Range(0, roadNodes.Count)];
            if (IsTooClose(node.position, marker)) continue;

            marker.position = node.position + Vector3.up * 0.3f;
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
        place.Clear();
        for (int i = 0; i < count; i++)
        {
            Transform node = roadNodes[Random.Range(0, roadNodes.Count)];
            roadNodes.Remove(node);
            node.tag = tag;
            place.Add(node);
            GameManager.inst.pool.Spawn(tag, node.position, Quaternion.Euler(90, 0, 0));
        }

        OnGasStationsInitialized?.Invoke();
    }

    #endregion
    /* ===================================================================== */
}