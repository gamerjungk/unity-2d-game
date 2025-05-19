using UnityEngine;
using System.Collections.Generic;

public class RoadGenerate : MonoBehaviour
{
    private Dictionary<Vector2Int, List<Vector2Int>> graph = new();
    private HashSet<Vector2Int> visited = new();
    private List<Vector2Int> startingRoads = new();

    void Start()
    {
        //ActivateAllIntersections();
        RandomizeRoads();
        BuildGraph();
        IdentifyStartingRoads();
        EnsureConnectivity();
    }

    void RandomizeRoads()
    {
        for (int i = 1; i <= 12; i++)
        {
            SetBlockActive($"Vertical_block{i}", Random.value > 0.5f);
            SetBlockActive($"Horizontal_block{i}", Random.value > 0.5f);
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

                AddEdge(pos, new(row, col - 1), $"Horizontal_block{row * 4 + col}");
                AddEdge(pos, new(row, col + 1), $"Horizontal_block{row * 4 + col + 1}");
                AddEdge(pos, new(row - 1, col), $"Vertical_block{(row - 1) * 4 + col + 1}");
                AddEdge(pos, new(row + 1, col), $"Vertical_block{row * 4 + col + 1}");
            }
        }
    }

    void AddEdge(Vector2Int from, Vector2Int to, string roadName)
    {
        if (graph.ContainsKey(to) && IsRoadActive(roadName))
            graph[from].Add(to);
    }

    void IdentifyStartingRoads()
    {
        int[] verticalIndices = { 1, 2, 3, 10, 11, 12 };
        int[] horizontalIndices = { 1, 4, 5, 8, 9, 12 };

        foreach (int i in verticalIndices)
            if (IsRoadActive($"Vertical_block{i}"))
                startingRoads.Add(new(i, -1));

        foreach (int i in horizontalIndices)
            if (IsRoadActive($"Horizontal_block{i}"))
                startingRoads.Add(new(-1, i));
    }

    void EnsureConnectivity()
    {
        visited.Clear();
        foreach (Vector2Int start in startingRoads)
            if (graph.ContainsKey(start))
                DFS(start);

        List<Vector2Int> disconnectedNodes = new();
        foreach (var node in graph.Keys)
            if (!visited.Contains(node))
                disconnectedNodes.Add(node);

        foreach (var node in disconnectedNodes)
            ConnectToNearestIntersection(node);
    }

    void DFS(Vector2Int node)
    {
        if (!graph.ContainsKey(node) || !visited.Add(node)) return;
        foreach (var neighbor in graph[node]) DFS(neighbor);
    }

    void ConnectToNearestIntersection(Vector2Int node)
    {
        Vector2Int closest = FindClosestActiveIntersection(node);
        if (closest != node)
        {
            ActivateRoadBetween(node, closest);
            graph[node].Add(closest);
            graph[closest].Add(node);
        }
    }

    Vector2Int FindClosestActiveIntersection(Vector2Int node)
    {
        Vector2Int closest = node;
        float minDistance = float.MaxValue;

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
            SetBlockActive($"Vertical_block{from.x * 4 + Mathf.Min(from.y, to.y) + 1}", true);
        else if (from.y == to.y)
            SetBlockActive($"Horizontal_block{Mathf.Min(from.x, to.x) * 4 + from.y + 1}", true);
    }

    void SetBlockActive(string name, bool active)
    {
        GameObject block = GameObject.Find(name);
        if (block != null) block.SetActive(active);
    }

    bool IsRoadActive(string roadName)
    {
        GameObject road = GameObject.Find(roadName);
        return road != null && road.activeSelf;
    }
}
