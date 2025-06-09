using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class DestinationManager : MonoBehaviour
{
    public static DestinationManager Instance { get; private set; }

    [Header("External refs")]
    [SerializeField] Transform player;
    [SerializeField] NavMeshSurface surface;      // RoadRoot 의 NavMeshSurface
    [SerializeField] Transform[] markers;      // 4개 파티클
    [SerializeField] DestinationUI_M destUI;

    /* ─ 내부 상태 ─ */
    readonly List<Transform> roadNodes = new();
    Vector3?[] pickupPos;                             // 마커별 최근 픽업 위치
    int currentIdx;
    public Transform CurrentTarget { get; private set; }

    /* ─────────────────────────────── */
    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        pickupPos = new Vector3?[markers.Length];
        RefreshRoadNodes();
        foreach (var m in markers) MoveMarkerRandom(m);

        SelectTarget(0);                              // 첫 목표
    }

    /* ================================================================= */
    /*                  ▼    UI / 트리거가 호출하는 API    ▼             */
    /* ================================================================= */
    public Transform[] Markers => markers;
    public Transform Player => player;

    public void SelectTarget(int idx)
    {
        if ((uint)idx >= (uint)markers.Length) return;
        currentIdx = idx;
        CurrentTarget = markers[idx];

        PathDrawer_m.Instance?.DrawPath(player, CurrentTarget);
    }

    /// MoneyTrigger → 플레이어가 현 목표 Collider 에 닿음
    public void ArrivedCurrentTarget()
    {
        if (CurrentTarget == null) return;

        bool isPickupStage = destUI.IsPickup(currentIdx);

        /* ───────── 픽업 도착 ───────── */
        if (isPickupStage)
        {
            pickupPos[currentIdx] = CurrentTarget.position;   // 픽업 좌표 기억
            destUI.ArrivedAt(currentIdx);                     // UI 토글(배달 단계)
            Debug.Log($"픽업 완료 ▸ '{currentIdx}' 배달 단계 전환");
        }
        /* ───────── 배달 도착 ───────── */
        else
        {
            if (pickupPos[currentIdx].HasValue)
            {
                float dist = Vector3.Distance(pickupPos[currentIdx].Value,
                                                CurrentTarget.position);
                int reward = Mathf.RoundToInt(dist * 100);

                GameDataManager.Instance.AddMoney(reward);
                Debug.Log($"배달 완료 ▸ {dist:F1} m  ➜  +{reward} ₩");
            }
            else
                Debug.LogWarning("배달 단계인데 픽업 좌표가 없습니다(논리 오류)");

            pickupPos[currentIdx] = null;            // 다음 사이클 준비
            destUI.ArrivedAt(currentIdx);            // 다시 픽업 단계
        }

        /* 목적지 새 위치로 이동 + 경로 갱신 */
        MoveMarkerRandom(CurrentTarget);
        PathDrawer_m.Instance?.DrawPath(player, CurrentTarget);
    }

    /* ================================================================= */
    /*                       ▼       마커 배치        ▼                  */
    /* ================================================================= */
    void RefreshRoadNodes()
    {
        roadNodes.Clear();
        foreach (var go in GameObject.FindGameObjectsWithTag("RoadNode"))
            if (NavMesh.SamplePosition(go.transform.position, out _, .3f, NavMesh.AllAreas))
                roadNodes.Add(go.transform);
    }

    void MoveMarkerRandom(Transform marker)
    {
        if (roadNodes.Count == 0) RefreshRoadNodes();
        if (roadNodes.Count == 0) return;

        Transform node = roadNodes[Random.Range(0, roadNodes.Count)];
        marker.position = node.position + Vector3.up * .3f;
        marker.GetComponent<MoneyTrigger>()?.ResetTrigger();
    }

    /// <summary>외부(도로 ON/OFF)에서 호출해 NavMesh를 즉시 재베이크</summary>
    public void RebuildNavMesh()
    {
        if (surface != null)
            surface.BuildNavMesh();
        else
            Debug.LogWarning("DestinationManager ▸ NavMeshSurface 참조가 없습니다.");
    }
}
