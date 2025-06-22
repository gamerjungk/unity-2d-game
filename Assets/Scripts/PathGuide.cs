using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;

public class PathGuide : MonoBehaviour
{
    public Transform player;
    public Transform destination;
    public Tilemap wallTilemap;
    private LineRenderer pathRenderer;
    public float arrivalDistance = 0.5f;
    private Vector2 lastPlayerPos;
    private Vector2 lastDestPos;
    public float minDistanceFromWall = 2f; // 벽과의 최소 거리 (타일 단위)

    void Start()
    {
        if (player == null || destination == null || wallTilemap == null)
        {
            Debug.LogError("PathGuide: 필요한 오브젝트가 연결되지 않았습니다!");
            return;
        }

        pathRenderer = GetComponent<LineRenderer>();
        if (pathRenderer == null)
        {
            pathRenderer = gameObject.AddComponent<LineRenderer>();
        }

        pathRenderer.startWidth = 0.3f;
        pathRenderer.endWidth = 0.3f;
        pathRenderer.material = new Material(Shader.Find("Sprites/Default"));
        pathRenderer.startColor = Color.red;
        pathRenderer.endColor = Color.red;
        pathRenderer.sortingOrder = 10; // 타일맵 위에 그려지게

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

    if (curvedPath.Count > 0)
    {
        // 첫 번째 점을 플레이어 바로 중심이 아니라 약간 뒤로 밀어줌
        Vector2 dir = (curvedPath[1] - curvedPath[0]).normalized;
        curvedPath[0] += dir * 0.3f; // 0.3 유닛 앞으로
    }

    pathRenderer.positionCount = curvedPath.Count;
    pathRenderer.SetPositions(curvedPath.Select(p => (Vector3)p).ToArray());
}


    List<Vector2> GenerateCurvedPath(List<Vector2> path)
    {
        if (path.Count < 2) return path;

        List<Vector2> curvedPoints = new List<Vector2>();
        float step = 0.05f; // 더 부드럽게 만들려면 step을 줄여야 함

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector2 p0 = i > 0 ? path[i - 1] : path[i];
            Vector2 p1 = path[i];
            Vector2 p2 = path[i + 1];
            Vector2 p3 = i + 2 < path.Count ? path[i + 2] : p2;

            for (float t = 0; t <= 1; t += step)
            {
                Vector2 point = CatmullRom(p0, p1, p2, p3, t);
                curvedPoints.Add(point);
            }
        }
        return curvedPoints;
    }

    Vector2 CatmullRom(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
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
        } while (IsObstacle(randomPos) || // 벽인지 확인
                 Vector2.Distance(randomPos, player.position) < 1f || // 플레이어와 너무 가까운지
                 !IsFarEnoughFromWalls(randomPos)); // 벽과 너무 가까운지

        destination.position = randomPos;
    }

    bool IsFarEnoughFromWalls(Vector2 position)
    {
        // 주변 타일 확인 (1타일 반경)
        Vector3Int centerCell = wallTilemap.WorldToCell(position);
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int checkCell = centerCell + new Vector3Int(x, y, 0);
                Vector2 checkPos = wallTilemap.CellToWorld(checkCell);
                if (wallTilemap.HasTile(checkCell))
                {
                    float distance = Vector2.Distance(position, checkPos);
                    if (distance < minDistanceFromWall)
                    {
                        return false; // 벽과 너무 가까움
                    }
                }
            }
        }
        return true; // 벽과 충분히 떨어짐
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
        cell + Vector3Int.right,
        cell + new Vector3Int(1, 1, 0),
        cell + new Vector3Int(-1, 1, 0),
        cell + new Vector3Int(1, -1, 0),
        cell + new Vector3Int(-1, -1, 0),
        };
    }

    bool IsObstacle(Vector2 position)
    {
        Vector3Int cell = wallTilemap.WorldToCell(position);
        return wallTilemap.HasTile(cell);
    }
}