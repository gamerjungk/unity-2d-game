using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;

/*
public class PathGuide : MonoBehaviour
{
    public Transform player; // 플레이어 오브젝트
    public Transform destination; // 목적지 오브젝트
    private LineRenderer lineRenderer; // 경로선(Line Renderer)
    public Tilemap wallTilemap; // 타일맵
    private Vector2 lastPlayerPos; // 마지막 플레이어 위치 저장
    private Vector2 lastDestPos; // 마지막 목적지 위치 저장
    public float arrivalDistance = 0.5f; // 도착으로 간주할 거리

    void Start()
    {
        // line Renderer 컴포넌트 가져오기
        lineRenderer = GetComponent<LineRenderer>();

        // 필수 있어야할 오브젝트가 모두 연결되있는지 확인
        if (player == null || destination == null || wallTilemap == null || lineRenderer == null)
        {
            Debug.LogError("PathGuide: 필요한 오브젝트가 연결되지 않았습니다!");
            return;
        }

        // 초기 플레이어, 목적지 위치 저장
        lastPlayerPos = player.position;
        lastDestPos = destination.position;

        // 첫 경로 업데이트 실행
        UpdatePath();
    }

    void Update()
    {
        // 플레이어가 목적지에 도착했는지 체크
        if (Vector2.Distance(player.position, destination.position) < arrivalDistance)
        {
            SetRandomDestination(); // 랜덤 new 목적지 설정
        }

        // 플레이어나 목적지가 이동했으면 경로 갱신
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
        // Astar 알고리즘을 이용해 경로 찾기
        List<Vector2> path = FindPathWithAStar(player.position, destination.position);

        // 경로를 line renderer에 적용
        lineRenderer.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            lineRenderer.SetPosition(i, path[i]);
        }
    }

    void SetRandomDestination()
    {
        // 타일맵의 경계 범위 가져오기
        BoundsInt bounds = wallTilemap.cellBounds;
        Vector3Int randomCell;
        Vector2 randomPos;

        // 이동 가능한 위치를 찾을 때까지 반복
        do
        {
            randomCell = new Vector3Int(
                Random.Range(bounds.xMin, bounds.xMax),
                Random.Range(bounds.yMin, bounds.yMax),
                0
            );
            randomPos = wallTilemap.CellToWorld(randomCell);
        } while (IsObstacle(randomPos) || Vector2.Distance(randomPos, player.position) < 1f);

        destination.position = randomPos; // 새 목적지 설정
        Debug.Log("새 목적지: " + randomPos);
    }

    List<Vector2> FindPathWithAStar(Vector2 startPos, Vector2 targetPos)
    {
        // 월드 좌표를 타일 좌표로 변환
        Vector3Int startCell = wallTilemap.WorldToCell(startPos);
        Vector3Int targetCell = wallTilemap.WorldToCell(targetPos);

        // A* 알고리즘에 필요한 리스트 및 딕셔너리 생성
        List<Vector3Int> openList = new List<Vector3Int> { startCell };
        HashSet<Vector3Int> closedList = new HashSet<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, float> gScore = new Dictionary<Vector3Int, float> { { startCell, 0 } };
        Dictionary<Vector3Int, float> fScore = new Dictionary<Vector3Int, float> { { startCell, Vector3Int.Distance(startCell, targetCell) } };

        // 경로 탐색 루프문
        while (openList.Count > 0)
        {
            // fScore가 가장 낮은 노드를 선택
            Vector3Int current = openList.OrderBy(n => fScore[n]).First();

            // 목적지에 도착하면 경로 변환
            if (current == targetCell)
                return ReconstructPath(cameFrom, current);

            openList.Remove(current);
            closedList.Add(current);

            // 현재 노드의 이웃 노드 탐색
            foreach (Vector3Int neighbor in GetNeighbors(current))
            {
                // 이미 방문한 노드이거나 장애물인 경우 무시
                if (closedList.Contains(neighbor) || IsObstacle(wallTilemap.CellToWorld(neighbor)))
                    continue;

                float tentativeGScore = gScore[current] + 1;
                if (!openList.Contains(neighbor))
                    openList.Add(neighbor);
                else if (tentativeGScore >= gScore[neighbor])
                    continue;

                // 경로 갱신
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Vector3Int.Distance(neighbor, targetCell);
            }
        }
        return new List<Vector2>(); // 경로를 찾지 못한 경우 빈 리스트 반환
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
        // 현재 위치에서 상하좌우 이웃 타일 변환
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
        // 해당 위치가 장애물인지 확인
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
            Debug.LogError("PathGuide: 필요한 오브젝트가 연결되지 않았습니다!");
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
        float step = 0.02f; // 더 부드럽게 (작을수록 촘촘)

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector2 p0 = i > 0 ? path[i - 1] : path[i]; // 이전 포인트
            Vector2 p1 = path[i]; // 현재 포인트
            Vector2 p2 = path[i + 1]; // 다음 포인트
            Vector2 p3 = i + 2 < path.Count ? path[i + 2] : p2; // 다음 다음 포인트

            // 제어점 부드럽게 조정
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