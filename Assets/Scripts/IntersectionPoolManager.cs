using UnityEngine;
using System.Collections.Generic;

public class RoadGraph : MonoBehaviour
{
    private Dictionary<Vector2Int, List<Vector2Int>> graph = new Dictionary<Vector2Int, List<Vector2Int>>();
    private HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
    
    void Start()
    {
        RandomizeIntersections();
        RandomizeRoads();
        BuildGraph();
        PrintGraph();
        EnsureConnectivity();
    }

    void RandomizeIntersections()
    {
        for (int i = 1; i <= 16; i++)
        {
            GameObject block = GameObject.Find($"Intersection_block{i}");
            if (block != null)
            {
                block.SetActive(Random.value > 0.5f);
            }
        }
    }

    void RandomizeRoads()
    {
        for (int i = 1; i <= 12; i++)
        {
            GameObject verticalBlock = GameObject.Find($"Vertical_block{i}");
            GameObject horizontalBlock = GameObject.Find($"Horizontal_block{i}");

            if (verticalBlock != null)
                verticalBlock.SetActive(Random.value > 0.5f);

            if (horizontalBlock != null)
                horizontalBlock.SetActive(Random.value > 0.5f);
        }
    }

    void BuildGraph()
    {
        graph.Clear();
        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                Vector2Int pos = new Vector2Int(row, col);
                GameObject intersection = GameObject.Find($"Intersection_block{row * 4 + col + 1}");
                
                if (intersection != null && intersection.activeSelf)
                {
                    graph[pos] = new List<Vector2Int>();
                    
                    // Add Horizontal connections
                    if (col > 0 && IsRoadActive($"Horizontal_block{row * 4 + col}"))
                        graph[pos].Add(new Vector2Int(row, col - 1));
                    if (col < 4 && IsRoadActive($"Horizontal_block{row * 4 + col + 1}"))
                        graph[pos].Add(new Vector2Int(row, col + 1));
                    
                    // Add Vertical connections
                    if (row > 0 && IsRoadActive($"Vertical_block{(row - 1) * 4 + col + 1}"))
                        graph[pos].Add(new Vector2Int(row - 1, col));
                    if (row < 4 && IsRoadActive($"Vertical_block{row * 4 + col + 1}"))
                        graph[pos].Add(new Vector2Int(row + 1, col));
                }
            }
        }
    }

    bool IsRoadActive(string roadName)
    {
        GameObject road = GameObject.Find(roadName);
        return road != null && road.activeSelf;
    }

    void PrintGraph()
    {
        Debug.Log("Graph Connections:");
        foreach (var node in graph)
        {
            string connections = "";
            foreach (var neighbor in node.Value)
            {
                connections += $"({neighbor.x}, {neighbor.y}) ";
            }
            Debug.Log($"Node ({node.Key.x}, {node.Key.y}) -> {connections}");
        }
    }

    void EnsureConnectivity()
    {
        visited.Clear();
        Vector2Int start = new Vector2Int(0, 0);
        DFS(start);

        List<Vector2Int> disconnectedNodes = new List<Vector2Int>();
        foreach (var node in graph.Keys)
        {
            if (!visited.Contains(node))
            {
                disconnectedNodes.Add(node);
            }
        }

        foreach (var node in disconnectedNodes)
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
            ActivateIntersection(node);
            ActivateRoadBetween(node, closest);
            graph[node] = new List<Vector2Int> { closest };
            graph[closest].Add(node);
        }
    }

    Vector2Int FindClosestActiveIntersection(Vector2Int node)
    {
        float minDistance = float.MaxValue;
        Vector2Int closest = node;

        foreach (var other in graph.Keys)
        {
            if (visited.Contains(other))
            {
                float dist = Vector2Int.Distance(node, other);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = other;
                }
            }
        }
        return closest;
    }

    void ActivateIntersection(Vector2Int node)
    {
        GameObject intersection = GameObject.Find($"Intersection_block{node.x * 4 + node.y + 1}");
        if (intersection != null)
        {
            intersection.SetActive(true);
        }
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
    }
}