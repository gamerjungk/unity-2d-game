using UnityEngine;
using System.Collections.Generic;

public class RoadGraph : MonoBehaviour
{
    private Dictionary<Vector2Int, List<Vector2Int>> graph = new();
    private HashSet<Vector2Int> visited = new();
    private List<Vector2Int> startingRoads = new();
    private Dictionary<string, GameObject> roadBlocks = new();
    public GameObject wallBlock_H;
    public GameObject wallBlock_V;

    void Awake()
    {
        CacheRoadBlocks();
    }

    void Start()
    {
        RandomizeRoads();
        BuildGraph();
        IdentifyStartingRoads();
        EnsureConnectivity();
    }

    void CacheRoadBlocks()
    {
        Transform[] allTransforms = GameObject.FindObjectsOfType<Transform>(true);
        foreach (Transform t in allTransforms)
        {
            if (t.name.StartsWith("Horizontal_block") || t.name.StartsWith("Vertical_block"))
            {
                roadBlocks[t.name] = t.gameObject;
            }
        }
    }

    void RandomizeRoads()
    {
        for (int i = 1; i <= 12; i++)
        {
            if (roadBlocks.TryGetValue($"Vertical_block{i}", out var v))
            {
                bool isActive = Random.value > 0.5f;
                v.SetActive(isActive);

                if (!isActive)
                {
                    PoolManager_wall.Instance.GetFromPool("WallBlock_V", v.transform.position, Quaternion.identity);
                }
            }

            if (roadBlocks.TryGetValue($"Horizontal_block{i}", out var h))
            {
                bool isActive = Random.value > 0.5f;
                h.SetActive(isActive);

                if (!isActive)
                {
                    PoolManager_wall.Instance.GetFromPool("WallBlock_H", h.transform.position, Quaternion.identity);
                }
            }
        }
    }

    void BuildGraph()
    {
        graph.Clear();
        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                Vector2Int pos = new(row, col);
                graph[pos] = new();

                if (col > 0 && IsRoadActive($"Horizontal_block{row * 4 + col}"))
                    graph[pos].Add(new(row, col - 1));
                if (col < 4 && IsRoadActive($"Horizontal_block{row * 4 + col + 1}"))
                    graph[pos].Add(new(row, col + 1));

                if (row > 0 && IsRoadActive($"Vertical_block{(row - 1) * 4 + col + 1}"))
                    graph[pos].Add(new(row - 1, col));
                if (row < 4 && IsRoadActive($"Vertical_block{row * 4 + col + 1}"))
                    graph[pos].Add(new(row + 1, col));
            }
        }
    }

    bool IsRoadActive(string roadName)
    {
        if (roadBlocks.TryGetValue(roadName, out var obj))
            return obj.activeSelf;
        return false;
    }

    void ActivateRoad(string name)
    {
        if (roadBlocks.TryGetValue(name, out var obj))
            obj.SetActive(true);
    }

    void IdentifyStartingRoads()
    {
        startingRoads.Clear();
        foreach (var node in graph.Keys)
        {
            if (graph[node].Count > 0)
                startingRoads.Add(node);
        }
    }

    void EnsureConnectivity()
    {
        visited.Clear();
        foreach (Vector2Int start in startingRoads)
        {
            DFS(start);
        }

        List<Vector2Int> disconnected = new();
        foreach (var node in graph.Keys)
        {
            if (!visited.Contains(node))
                disconnected.Add(node);
        }

        foreach (var node in disconnected)
        {
            ConnectToNearestIntersection(node);
        }
    }

    void DFS(Vector2Int node)
    {
        if (!graph.ContainsKey(node) || visited.Contains(node)) return;
        visited.Add(node);

        foreach (var neighbor in graph[node])
        {
            DFS(neighbor);
        }
    }

void ConnectToNearestIntersection(Vector2Int node)
{
    Vector2Int closest = FindClosestActiveIntersection(node);
    if (closest != node)
    {
        ActivateRoadBetween(node, closest);
        BuildGraph();
        DFS(node);
    }
}

    Vector2Int FindClosestActiveIntersection(Vector2Int node)
    {
        float minDistance = float.MaxValue;
        Vector2Int closest = node;

        foreach (var other in visited)
        {
            float dist = Vector2Int.Distance(node, other);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = other;
            }
        }
        return closest;
    }

    void ActivateRoadBetween(Vector2Int from, Vector2Int to)
    {
        if (from.x == to.x)
        {
            int minY = Mathf.Min(from.y, to.y);
            GameObject road = GameObject.Find($"Vertical_block{from.x * 4 + minY + 1}");
            if (road != null) road.SetActive(true);
        }
        else if (from.y == to.y)
        {
            int minX = Mathf.Min(from.x, to.x);
            GameObject road = GameObject.Find($"Horizontal_block{minX * 4 + from.y + 1}");
            if (road != null) road.SetActive(true);
        }

        // 그래프에도 연결 추가
        if (!graph[from].Contains(to)) graph[from].Add(to);
        if (!graph[to].Contains(from)) graph[to].Add(from);
    }

}