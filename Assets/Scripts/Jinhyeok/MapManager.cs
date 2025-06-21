using CiDy;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public CiDyGraph graph;
    public int nodeCount = 10; // ������ ��� ����
    public float areaSize = 100f; // ������ ��ġ ����
    public float nodeScale = 5f;

    // ����� �Ķ���� (�⺻���� CiDy ������ ����)
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
        graph = FindObjectOfType<CiDyGraph>();
        if (graph == null)
        {
            Debug.LogError("CiDyGraph�� ������� �ʾҽ��ϴ�!");
            return;
        }

        List<CiDyNode> nodes = new List<CiDyNode>();

        // ������ (0,0,0) ��� ����
        CiDyNode firstNode = graph.NewMasterNode(Vector3.zero, nodeScale);
        nodes.Add(firstNode);

        // ������ ��ġ�� ��� ����
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

        // ��� ����: �ܼ��� ���� ��� (���� ���� �� ��õ� ����)
        for (int i = 1; i < nodes.Count; i++)
        {
            CiDyNode a = nodes[i];
            CiDyNode b = nodes[Random.Range(0, i)]; // ���������� ��� �� ���� ����

            bool success = graph.ConnectNodes(
                a, b, laneWidth, roadSegmentLength, flattenAmount,
                flipStopSign, roadLevel, laneType,
                leftShoulderWidth, centerWidth, rightShoulderWidth
            );

            if (!success)
            {
                Debug.LogWarning($"��� ���� ����: {a.name} - {b.name}");
            }
        }

        Debug.Log($"�� {nodes.Count}���� ���� ������ �����Ǿ����ϴ�.");
    }
}