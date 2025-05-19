using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;

/*
public class PathGuide : MonoBehaviour
{
    public Transform player; // �÷��̾� ������Ʈ
    public Transform destination; // ������ ������Ʈ
    private LineRenderer lineRenderer; // ��μ�(Line Renderer)
    public Tilemap wallTilemap; // Ÿ�ϸ�
    private Vector2 lastPlayerPos; // ������ �÷��̾� ��ġ ����
    private Vector2 lastDestPos; // ������ ������ ��ġ ����
    public float arrivalDistance = 0.5f; // �������� ������ �Ÿ�

    void Start()
    {
        // line Renderer ������Ʈ ��������
        lineRenderer = GetComponent<LineRenderer>();

        // �ʼ� �־���� ������Ʈ�� ��� ������ִ��� Ȯ��
        if (player == null || destination == null || wallTilemap == null || lineRenderer == null)
        {
            Debug.LogError("PathGuide: �ʿ��� ������Ʈ�� ������� �ʾҽ��ϴ�!");
            return;
        }

        // �ʱ� �÷��̾�, ������ ��ġ ����
        lastPlayerPos = player.position;
        lastDestPos = destination.position;

        // ù ��� ������Ʈ ����
        UpdatePath();
    }

    void Update()
    {
        // �÷��̾ �������� �����ߴ��� üũ
        if (Vector2.Distance(player.position, destination.position) < arrivalDistance)
        {
            SetRandomDestination(); // ���� new ������ ����
        }

        // �÷��̾ �������� �̵������� ��� ����
        if (Vector2.Distance(lastPlayerPos, player.position) > 0.1f ||
            Vector2.Distance(lastDestPos, destination.position) > 0.1f)
        {
            UpdatePath();
            lastPlayerPos = player.position;
            lastDestPos = destination.position;
        }
    }

    void UpdatePath()
    {
        // Astar �˰����� �̿��� ��� ã��
        List<Vector2> path = FindPathWithAStar(player.position, destination.position);

        // ��θ� line renderer�� ����
        lineRenderer.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            lineRenderer.SetPosition(i, path[i]);
        }
    }

    void SetRandomDestination()
    {
        // Ÿ�ϸ��� ��� ���� ��������
        BoundsInt bounds = wallTilemap.cellBounds;
        Vector3Int randomCell;
        Vector2 randomPos;

        // �̵� ������ ��ġ�� ã�� ������ �ݺ�
        do
        {
            randomCell = new Vector3Int(
                Random.Range(bounds.xMin, bounds.xMax),
                Random.Range(bounds.yMin, bounds.yMax),
                0
            );
            randomPos = wallTilemap.CellToWorld(randomCell);
        } while (IsObstacle(randomPos) || Vector2.Distance(randomPos, player.position) < 1f);

        destination.position = randomPos; // �� ������ ����
        Debug.Log("�� ������: " + randomPos);
    }

    List<Vector2> FindPathWithAStar(Vector2 startPos, Vector2 targetPos)
    {
        // ���� ��ǥ�� Ÿ�� ��ǥ�� ��ȯ
        Vector3Int startCell = wallTilemap.WorldToCell(startPos);
        Vector3Int targetCell = wallTilemap.WorldToCell(targetPos);

        // A* �˰��� �ʿ��� ����Ʈ �� ��ųʸ� ����
        List<Vector3Int> openList = new List<Vector3Int> { startCell };
        HashSet<Vector3Int> closedList = new HashSet<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, float> gScore = new Dictionary<Vector3Int, float> { { startCell, 0 } };
        Dictionary<Vector3Int, float> fScore = new Dictionary<Vector3Int, float> { { startCell, Vector3Int.Distance(startCell, targetCell) } };

        // ��� Ž�� ������
        while (openList.Count > 0)
        {
            // fScore�� ���� ���� ��带 ����
            Vector3Int current = openList.OrderBy(n => fScore[n]).First();

            // �������� �����ϸ� ��� ��ȯ
            if (current == targetCell)
                return ReconstructPath(cameFrom, current);

            openList.Remove(current);
            closedList.Add(current);

            // ���� ����� �̿� ��� Ž��
            foreach (Vector3Int neighbor in GetNeighbors(current))
            {
                // �̹� �湮�� ����̰ų� ��ֹ��� ��� ����
                if (closedList.Contains(neighbor) || IsObstacle(wallTilemap.CellToWorld(neighbor)))
                    continue;

                float tentativeGScore = gScore[current] + 1;
                if (!openList.Contains(neighbor))
                    openList.Add(neighbor);
                else if (tentativeGScore >= gScore[neighbor])
                    continue;

                // ��� ����
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Vector3Int.Distance(neighbor, targetCell);
            }
        }
        return new List<Vector2>(); // ��θ� ã�� ���� ��� �� ����Ʈ ��ȯ
    }

    List<Vector2> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        List<Vector2> path = new List<Vector2> { wallTilemap.CellToWorld(current) };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, wallTilemap.CellToWorld(current));
        }
        return path;
    }

    Vector3Int[] GetNeighbors(Vector3Int cell)
    {
        // ���� ��ġ���� �����¿� �̿� Ÿ�� ��ȯ
        return new Vector3Int[]
        {
            cell + Vector3Int.up,
            cell + Vector3Int.down,
            cell + Vector3Int.left,
            cell + Vector3Int.right
        };
    }

    bool IsObstacle(Vector2 position)
    {
        // �ش� ��ġ�� ��ֹ����� Ȯ��
        Vector3Int cell = wallTilemap.WorldToCell(position);
        return wallTilemap.HasTile(cell);
    }
}*/


public class PathGuide : MonoBehaviour
{
    public Transform player;
    public Transform destination;
    public Tilemap wallTilemap;
    private LineRenderer pathRenderer;
    public float arrivalDistance = 0.5f;
    private Vector2 lastPlayerPos;
    private Vector2 lastDestPos;

    void Start()
    {
        if (player == null || destination == null || wallTilemap == null)
        {
            Debug.LogError("PathGuide: �ʿ��� ������Ʈ�� ������� �ʾҽ��ϴ�!");
            return;
        }

        pathRenderer = gameObject.AddComponent<LineRenderer>();
        pathRenderer.startWidth = 0.1f;
        pathRenderer.endWidth = 0.1f;
        pathRenderer.material = new Material(Shader.Find("Sprites/Default"));
        pathRenderer.startColor = Color.white;
        pathRenderer.endColor = Color.white;

        lastPlayerPos = player.position;
        lastDestPos = destination.position;
        UpdatePath();
    }

    void Update()
    {
        if (Vector2.Distance(player.position, destination.position) < arrivalDistance)
        {
            SetRandomDestination();
        }

        if (Vector2.Distance(lastPlayerPos, player.position) > 0.1f ||
            Vector2.Distance(lastDestPos, destination.position) > 0.1f)
        {
            UpdatePath();
            lastPlayerPos = player.position;
            lastDestPos = destination.position;
        }
    }

    void UpdatePath()
    {
        List<Vector2> path = FindPathWithAStar(player.position, destination.position);
        List<Vector2> curvedPath = GenerateCurvedPath(path);

        pathRenderer.positionCount = curvedPath.Count;
        pathRenderer.SetPositions(curvedPath.Select(p => (Vector3)p).ToArray());
    }

    List<Vector2> GenerateCurvedPath(List<Vector2> path)
    {
        if (path.Count < 2) return path;

        List<Vector2> curvedPoints = new List<Vector2>();
        float step = 0.02f; // �� �ε巴�� (�������� ����)

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector2 p0 = i > 0 ? path[i - 1] : path[i]; // ���� ����Ʈ
            Vector2 p1 = path[i]; // ���� ����Ʈ
            Vector2 p2 = path[i + 1]; // ���� ����Ʈ
            Vector2 p3 = i + 2 < path.Count ? path[i + 2] : p2; // ���� ���� ����Ʈ

            // ������ �ε巴�� ����
            Vector2 c1 = p1 + (p2 - p0).normalized * Vector2.Distance(p1, p2) * 0.3f;
            Vector2 c2 = p2 - (p3 - p1).normalized * Vector2.Distance(p1, p2) * 0.3f;

            for (float t = 0; t <= 1; t += step)
            {
                Vector2 point = CubicBezier(p1, c1, c2, p2, t);
                curvedPoints.Add(point);
            }
        }
        return curvedPoints;
    }

    Vector2 CubicBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector2 point = uuu * p0 + 3 * uu * t * p1 + 3 * u * tt * p2 + ttt * p3;
        return point;
    }

    void SetRandomDestination()
    {
        BoundsInt bounds = wallTilemap.cellBounds;
        Vector3Int randomCell;
        Vector2 randomPos;

        do
        {
            randomCell = new Vector3Int(
                Random.Range(bounds.xMin, bounds.xMax),
                Random.Range(bounds.yMin, bounds.yMax),
                0
            );
            randomPos = wallTilemap.CellToWorld(randomCell);
        } while (IsObstacle(randomPos) || Vector2.Distance(randomPos, player.position) < 1f);

        destination.position = randomPos;
    }

    List<Vector2> FindPathWithAStar(Vector2 startPos, Vector2 targetPos)
    {
        Vector3Int startCell = wallTilemap.WorldToCell(startPos);
        Vector3Int targetCell = wallTilemap.WorldToCell(targetPos);

        List<Vector3Int> openList = new List<Vector3Int> { startCell };
        HashSet<Vector3Int> closedList = new HashSet<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, float> gScore = new Dictionary<Vector3Int, float> { { startCell, 0 } };
        Dictionary<Vector3Int, float> fScore = new Dictionary<Vector3Int, float> { { startCell, Vector3Int.Distance(startCell, targetCell) } };

        while (openList.Count > 0)
        {
            Vector3Int current = openList.OrderBy(n => fScore[n]).First();
            if (current == targetCell)
                return ReconstructPath(cameFrom, current);

            openList.Remove(current);
            closedList.Add(current);

            foreach (Vector3Int neighbor in GetNeighbors(current))
            {
                if (closedList.Contains(neighbor) || IsObstacle(wallTilemap.CellToWorld(neighbor)))
                    continue;

                float tentativeGScore = gScore[current] + 1;
                if (!openList.Contains(neighbor))
                    openList.Add(neighbor);
                else if (tentativeGScore >= gScore[neighbor])
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Vector3Int.Distance(neighbor, targetCell);
            }
        }
        return new List<Vector2>();
    }

    List<Vector2> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        List<Vector2> path = new List<Vector2> { wallTilemap.CellToWorld(current) };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, wallTilemap.CellToWorld(current));
        }
        return path;
    }

    Vector3Int[] GetNeighbors(Vector3Int cell)
    {
        return new Vector3Int[]
        {
            cell + Vector3Int.up,
            cell + Vector3Int.down,
            cell + Vector3Int.left,
            cell + Vector3Int.right
        };
    }

    bool IsObstacle(Vector2 position)
    {
        Vector3Int cell = wallTilemap.WorldToCell(position);
        return wallTilemap.HasTile(cell);
    }
}