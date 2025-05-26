using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;

public class DestinationManager : MonoBehaviour
{
    public static DestinationManager Instance { get; private set; }

    [Header("External refs")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform destinationMarker;
    [SerializeField] private NavMeshSurface surface;

    public Transform DestinationMarker => destinationMarker;

    public void RebuildNavMesh() => surface.BuildNavMesh();

    private readonly List<Transform> activeRoadNodes = new();

    //추가
    private Vector3 lastDestinationPosition;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        RefreshRoadNodeList();

        //추가
        lastDestinationPosition = player.position;

        MoveDestination();          // 목적지 & NavMesh 세팅
    }

    /** 첫 프레임 끝난 뒤 PathDrawer 호출 **/
    IEnumerator Start()
    {
        yield return null;          // ① 한 프레임 대기 (PathDrawer Awake 확실)
        PathDrawer_m.Instance.DrawPath(player, destinationMarker);
    }

    public void RefreshRoadNodeList()
    {
        activeRoadNodes.Clear();
        foreach (var go in GameObject.FindGameObjectsWithTag("RoadNode"))
        {
            if (!go.activeInHierarchy) continue;                // 꺼진 노드는 제외

            // NavMesh 위에 실제로 올라와 있는지 확인
            if (NavMesh.SamplePosition(go.transform.position, out var hit, 0.2f, NavMesh.AllAreas))
                activeRoadNodes.Add(go.transform);
        }
    }


    public void MoveDestination()
    {
        RefreshRoadNodeList();
        if (activeRoadNodes.Count == 0)
        {
            Debug.LogError("RoadNode가 하나도 없습니다!");
            return;
        }

        Transform next = activeRoadNodes[Random.Range(0, activeRoadNodes.Count)];
        destinationMarker.position = next.position + Vector3.up * 0.3f;

        destinationMarker.GetComponent<MoneyTrigger>()?.ResetTrigger();

        surface.BuildNavMesh();   // 경로 계산 전에 반드시 NavMesh 준비
        if (PathDrawer_m.Instance != null)
            PathDrawer_m.Instance.DrawPath(player, destinationMarker);
    }

    //[추가] 목적지 도달 시 호출 → 보상 계산 및 다음 목적지 이동
    public void OnDestinationReached(Vector3 currentDestination)
    {
        float distance = Vector3.Distance(lastDestinationPosition, currentDestination);
        int reward = Mathf.FloorToInt(distance + 0.5f) * 100;

        GameManager.inst.AddMoney(reward);
 
        Debug.Log($"보상 지급: {reward} (거리: {distance:F2})");


        // 기준 위치 갱신
        lastDestinationPosition = currentDestination;

        MoveDestination();  // 다음 목적지 설정
    }
}
