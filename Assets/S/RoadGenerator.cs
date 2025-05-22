using UnityEngine;
using UnityEngine.Tilemaps;

public class RoadGenerator : MonoBehaviour
{
    public Tilemap roadTilemap; // 도로 타일맵
    public TileBase roadTile; // 도로 타일
    public int width = 20; // 맵 너비
    public int height = 20; // 맵 높이
    public float roadDensity = 0.4f; // 도로 생성 확률 (0~1)

    private bool[,] roadGrid; // 도로 맵 데이터 (true: 도로, false: 빈칸)

    void Start()
    {
        if (roadTilemap == null || roadTile == null)
        {
            Debug.LogError("RoadGenerator: 필요한 오브젝트가 연결되지 않았습니다!");
            return;
        }

        GenerateRoadMap();
        EnsureOuterRoad();
        EnsureConnectivity();
        ApplyRoadToTilemap();
    }

    void GenerateRoadMap()
    {
        roadGrid = new bool[width, height];

        // 랜덤으로 도로 생성
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                roadGrid[x, y] = Random.value < roadDensity;
            }
        }
    }

    void EnsureOuterRoad()
    {
        // 외곽을 도로로 채움
        for (int x = 0; x < width; x++)
        {
            roadGrid[x, 0] = true; // 맨 아래
            roadGrid[x, height - 1] = true; // 맨 위
        }
        for (int y = 0; y < height; y++)
        {
            roadGrid[0, y] = true; // 맨 왼쪽
            roadGrid[width - 1, y] = true; // 맨 오른쪽
        }
    }

    void EnsureConnectivity()
    {
        // Flood Fill로 연결성 확인 및 보장
        bool[,] visited = new bool[width, height];
        Vector2Int startPos = new Vector2Int(0, 0); // 외곽에서 시작
        FloodFill(startPos.x, startPos.y, visited);

        // 연결되지 않은 도로를 제거
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (roadGrid[x, y] && !visited[x, y])
                {
                    roadGrid[x, y] = false; // 연결되지 않은 도로는 제거
                }
            }
        }

        // 외곽 도로를 기준으로 다시 연결성 보장
        ConnectIsolatedRegions();
    }

    void FloodFill(int x, int y, bool[,] visited)
    {
        if (x < 0 || x >= width || y < 0 || y >= height || visited[x, y] || !roadGrid[x, y])
            return;

        visited[x, y] = true;

        FloodFill(x + 1, y, visited);
        FloodFill(x - 1, y, visited);
        FloodFill(x, y + 1, visited);
        FloodFill(x, y - 1, visited);
    }

    void ConnectIsolatedRegions()
    {
        bool[,] visited = new bool[width, height];
        FloodFill(0, 0, visited); // 외곽에서 시작

        // 연결되지 않은 도로가 있으면 외곽 도로와 연결
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (roadGrid[x, y] && !visited[x, y])
                {
                    // 가장 가까운 외곽 도로로 연결 경로 생성
                    ConnectToNearestRoad(x, y);
                }
            }
        }
    }

    void ConnectToNearestRoad(int startX, int startY)
    {
        // 간단하게 수평/수직 경로로 연결
        int nearestX = startX < width / 2 ? 0 : width - 1; // 좌우 외곽 중 가까운 쪽
        for (int x = Mathf.Min(startX, nearestX); x <= Mathf.Max(startX, nearestX); x++)
        {
            roadGrid[x, startY] = true;
        }
        // 연결된 외곽 도로에서 시작점까지 수직 경로 추가
        if (nearestX == 0)
        {
            for (int y = 0; y <= startY; y++)
            {
                roadGrid[0, y] = true;
            }
        }
        else
        {
            for (int y = startY; y < height; y++)
            {
                roadGrid[width - 1, y] = true;
            }
        }
    }

    void ApplyRoadToTilemap()
    {
        roadTilemap.ClearAllTiles();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (roadGrid[x, y])
                {
                    roadTilemap.SetTile(new Vector3Int(x, y, 0), roadTile);
                }
            }
        }
    }
}