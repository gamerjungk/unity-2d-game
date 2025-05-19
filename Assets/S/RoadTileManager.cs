using UnityEngine;
using System.Collections.Generic;

public class RoadTileManager : MonoBehaviour
{
    public static RoadTileManager Instance;

    public List<Transform> roadPoints = new List<Transform>();

    void Awake()
    {
        Instance = this;

        // "Road" �±װ� ���� ������Ʈ�� ã�� ����Ʈ�� ����
        GameObject[] roads = GameObject.FindGameObjectsWithTag("Road");
        foreach (var road in roads)
        {
            roadPoints.Add(road.transform);
        }
    }

    public Transform GetRandomRoadPoint()
    {
        return roadPoints[Random.Range(0, roadPoints.Count)];
    }
}
