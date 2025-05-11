using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public float nodeRadius = 1f;
    public LayerMask unwalkableMask;

    [Header("Path Settings")]
    public Transform player;
    public Transform destinationObject; // 씬 오브젝트 참조
    public LineRenderer lineRenderer;

    private Node[,] grid;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;
    private Vector2 gridWorldSize;

    void Start()
    {
        FitColliderToRoads();

        nodeDiameter = nodeRadius * 2;

        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            gridWorldSize = new Vector2(box.size.x * transform.localScale.x, box.size.z * transform.localScale.z);
        }
        else
        {
            Debug.LogError("PathManager에 BoxCollider가 필요합니다.");
        }

        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();

        SpawnNewDestination();
    }

    void FitColliderToRoads()
    {
        GameObject[] roads = GameObject.FindGameObjectsWithTag("Road");

        if (roads.Length == 0)
        {
            Debug.LogWarning("Road 태그 오브젝트 없음!");
            return;
        }

        Bounds bounds = new Bounds(roads[0].transform.position, Vector3.zero);

        foreach (GameObject road in roads)
        {
            Renderer rend = road.GetComponent<Renderer>();
            if (rend != null)
                bounds.Encapsulate(rend.bounds);
            else
                bounds.Encapsulate(road.transform.position);
        }

        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null)
            box = gameObject.AddComponent<BoxCollider>();

        box.center = transform.InverseTransformPoint(bounds.center);
        box.size = bounds.size;
    }

    void Update()
    {
        if (destinationObject != null)
        {
            List<Vector3> path = FindPath(player.position, destinationObject.position);
            DrawSmoothPath(path);

            float dist = Vector3.Distance(player.position, destinationObject.position);
            if (dist < 1.0f)
            {
                SpawnNewDestination();
            }
        }
    }

    public void SpawnNewDestination()
    {
        List<Node> roadNodes = new List<Node>();
        foreach (Node node in grid)
        {
            if (!node.walkable) continue;

            RaycastHit hit;
            Vector3 rayOrigin = node.worldPosition + Vector3.up * 2f;
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 5f))
            {
                if (hit.collider.CompareTag("Road"))
                {
                    roadNodes.Add(node);
                }
            }
        }

        if (roadNodes.Count == 0)
        {
            Debug.LogWarning("도로 위의 목적지 후보 노드가 없습니다!");
            return;
        }

        Node randNode = roadNodes[Random.Range(0, roadNodes.Count)];
        Vector3 spawnPos = randNode.worldPosition;
        Vector3 playerPos = player.position;
        spawnPos.y = 0.25f;
        playerPos.y = 0.25f;

        destinationObject.position = spawnPos;

        lineRenderer.SetPositions(new Vector3[] { playerPos, spawnPos });
        Debug.Log($"[Destination Spawned] {spawnPos} / 후보 노드 수: {roadNodes.Count}");
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = NodeFromWorldPoint(startPos);
        Node targetNode = NodeFromWorldPoint(targetPos);

        if (!startNode.walkable || !targetNode.walkable)
        {
            Debug.LogWarning("시작 또는 목적지 노드가 walkable하지 않습니다.");
            return new List<Vector3>();
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
                return RetracePath(startNode, targetNode);

            foreach (Node neighbor in GetNeighbours(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        Debug.LogWarning("경로를 찾을 수 없습니다.");
        return new List<Vector3>();
    }

    void DrawSmoothPath(List<Vector3> path)
    {
        if (path == null || path.Count < 2)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        List<Vector3> smoothPath = new List<Vector3>();
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 p0 = path[i];
            Vector3 p1 = path[i + 1];
            for (float t = 0; t < 1f; t += 0.1f)
            {
                Vector3 point = Vector3.Lerp(p0, p1, t);
                point.y = 0.1f;
                smoothPath.Add(point);
            }
        }

        smoothPath.Add(path[path.Count - 1]);

        lineRenderer.positionCount = smoothPath.Count;
        lineRenderer.SetPositions(smoothPath.ToArray());
    }

    List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        List<Vector3> waypoints = new List<Vector3>();
        foreach (Node node in path)
        {
            waypoints.Add(node.worldPosition);
        }
        return waypoints;
    }

    Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = Mathf.Clamp01((worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    neighbours.Add(grid[checkX, checkY]);
            }
        }
        return neighbours;
    }

    int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);

        return dstX > dstY ? 14 * dstY + 10 * (dstX - dstY) : 14 * dstX + 10 * (dstY - dstX);
    }
}

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }

    public int fCost => gCost + hCost;
}
