using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathGuide2 : MonoBehaviour
{
    public static PathGuide2 Instance;

    public LineRenderer lineRenderer;
    public Transform destinationIndicator;
    public float nodeCheckRadius = 0.1f;

    private List<GameObject> activeRoads = new List<GameObject>();

    void Awake()
    {
        Instance = this;

        var allRoads = GameObject.FindGameObjectsWithTag("Road").ToList();
        foreach (GameObject road in allRoads)
        {
            bool disable = Random.value < 0.2f;
            road.SetActive(!disable);
            if (!disable)
                activeRoads.Add(road);
        }
    }

    public void FindPath(Vector3 startPos, Vector3 endPos)
    {
        GameObject startNode = FindClosestRoad(startPos);
        GameObject endNode = FindClosestRoad(endPos);

        if (startNode == null || endNode == null)
        {
            Debug.LogWarning("유효한 시작 또는 목적 도로가 없습니다.");
            lineRenderer.positionCount = 0;
            return;
        }

        Dictionary<GameObject, GameObject> cameFrom = new();
        Dictionary<GameObject, float> gScore = new();
        Dictionary<GameObject, float> fScore = new();
        List<GameObject> openSet = new() { startNode };

        foreach (var node in activeRoads)
        {
            gScore[node] = float.MaxValue;
            fScore[node] = float.MaxValue;
        }

        gScore[startNode] = 0;
        fScore[startNode] = Vector3.Distance(startNode.transform.position, endNode.transform.position);

        while (openSet.Count > 0)
        {
            var current = openSet.OrderBy(n => fScore[n]).First();

            if (current == endNode)
            {
                List<Vector3> path = new();
                while (cameFrom.ContainsKey(current))
                {
                    path.Add(current.transform.position + Vector3.up * 0.1f);
                    current = cameFrom[current];
                }
                path.Add(startNode.transform.position + Vector3.up * 0.1f);
                path.Reverse();
                DrawPath(path);
                return;
            }

            openSet.Remove(current);
            var neighbors = FindNeighbors(current);

            foreach (var neighbor in neighbors)
            {
                float tentativeG = gScore[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);
                if (tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Vector3.Distance(neighbor.transform.position, endNode.transform.position);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        Debug.Log("경로를 찾을 수 없습니다.");
        lineRenderer.positionCount = 0;
    }

    void DrawPath(List<Vector3> path)
    {
        if (path == null || path.Count == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        lineRenderer.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            lineRenderer.SetPosition(i, path[i]);
        }
    }

    public GameObject GetRandomActiveRoad()
    {
        if (activeRoads.Count == 0) return null;
        return activeRoads[Random.Range(0, activeRoads.Count)];
    }

    GameObject FindClosestRoad(Vector3 pos)
    {
        return activeRoads
            .OrderBy(r => Vector3.Distance(pos, r.transform.position))
            .FirstOrDefault();
    }

    List<GameObject> FindNeighbors(GameObject current)
    {
        List<GameObject> neighbors = new();

        foreach (var road in activeRoads)
        {
            if (road == current) continue;
            float dist = Vector3.Distance(road.transform.position, current.transform.position);
            if (dist <= 15f)
            {
                neighbors.Add(road);
            }
        }

        return neighbors;
    }

    public void SetDestination(Transform player)
    {
        GameObject newTarget = GetRandomActiveRoad();
        if (newTarget == null) return;

        Renderer rend = newTarget.GetComponent<Renderer>();
        Vector3 centerPos = rend != null ? rend.bounds.center : newTarget.transform.position;

        centerPos.y = 0.5f;
        FindPath(player.position, centerPos);
        destinationIndicator.position = centerPos;
    }
}
