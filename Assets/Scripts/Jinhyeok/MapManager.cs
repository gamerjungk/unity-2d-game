using CiDy;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public CiDyGraph graph;
    public int nodeCount = 10; // 생성할 노드 개수
    public float areaSize = 100f; // 무작위 배치 범위
    public float nodeScale = 5f;

    // 연결용 파라미터 (기본값은 CiDy 에디터 기준)
    public float laneWidth = 5f;
    public int roadSegmentLength = 5;
    public int flattenAmount = 1;
    public bool flipStopSign = false;
    public CiDyRoad.RoadLevel roadLevel;
    public CiDyRoad.LaneType laneType;
    public float leftShoulderWidth = 0.5f;
    public float centerWidth = 0.5f;
    public float rightShoulderWidth = 0.5f;

    void Start()
    {
        graph = Object.FindFirstObjectByType<CiDyGraph>();
        if (graph == null)
        {
            Debug.LogError("CiDyGraph가 연결되지 않았습니다!");
            return;
        }

        List<CiDyNode> nodes = new List<CiDyNode>();

        // 기준점 (0,0,0) 노드 생성
        CiDyNode firstNode = graph.NewMasterNode(Vector3.zero, nodeScale);
        nodes.Add(firstNode);

        // 무작위 위치에 노드 생성
        for (int i = 0; i < nodeCount - 1; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-areaSize, areaSize),
                0f,
                Random.Range(-areaSize, areaSize)
            );

            CiDyNode node = graph.NewMasterNode(pos, nodeScale);
            if (node != null)
            {
                nodes.Add(node);
            }
        }

        // 노드 연결: 단순한 연결 방식 (연결 실패 시 재시도 가능)
        for (int i = 1; i < nodes.Count; i++)
        {
            CiDyNode a = nodes[i];
            CiDyNode b = nodes[Random.Range(0, i)]; // 이전까지의 노드 중 랜덤 선택

            bool success = graph.ConnectNodes(
                a, b, laneWidth, roadSegmentLength, flattenAmount,
                flipStopSign, roadLevel, laneType,
                leftShoulderWidth, centerWidth, rightShoulderWidth
            );

            if (!success)
            {
                Debug.LogWarning($"노드 연결 실패: {a.name} - {b.name}");
            }
        }

        Debug.Log($"총 {nodes.Count}개의 노드와 연결이 생성되었습니다.");
    }
}