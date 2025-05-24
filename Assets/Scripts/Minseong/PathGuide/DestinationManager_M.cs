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

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        RefreshRoadNodeList();
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

        surface.BuildNavMesh();   // 경로 계산 전에 반드시 NavMesh 준비
        if (PathDrawer_m.Instance != null)
            PathDrawer_m.Instance.DrawPath(player, destinationMarker);
    }

   
}
