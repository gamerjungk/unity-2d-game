using UnityEngine;
using UnityEngine.Tilemaps;

public class RoadGenerator : MonoBehaviour
{
    public Tilemap roadTilemap; // ���� Ÿ�ϸ�
    public TileBase roadTile; // ���� Ÿ��
    public int width = 20; // �� �ʺ�
    public int height = 20; // �� ����
    public float roadDensity = 0.4f; // ���� ���� Ȯ�� (0~1)

    private bool[,] roadGrid; // ���� �� ������ (true: ����, false: ��ĭ)

    void Start()
    {
        if (roadTilemap == null || roadTile == null)
        {
            Debug.LogError("RoadGenerator: �ʿ��� ������Ʈ�� ������� �ʾҽ��ϴ�!");
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

        // �������� ���� ����
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
        // �ܰ��� ���η� ä��
        for (int x = 0; x < width; x++)
        {
            roadGrid[x, 0] = true; // �� �Ʒ�
            roadGrid[x, height - 1] = true; // �� ��
        }
        for (int y = 0; y < height; y++)
        {
            roadGrid[0, y] = true; // �� ����
            roadGrid[width - 1, y] = true; // �� ������
        }
    }

    void EnsureConnectivity()
    {
        // Flood Fill�� ���Ἲ Ȯ�� �� ����
        bool[,] visited = new bool[width, height];
        Vector2Int startPos = new Vector2Int(0, 0); // �ܰ����� ����
        FloodFill(startPos.x, startPos.y, visited);

        // ������� ���� ���θ� ����
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (roadGrid[x, y] && !visited[x, y])
                {
                    roadGrid[x, y] = false; // ������� ���� ���δ� ����
                }
            }
        }

        // �ܰ� ���θ� �������� �ٽ� ���Ἲ ����
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
        FloodFill(0, 0, visited); // �ܰ����� ����

        // ������� ���� ���ΰ� ������ �ܰ� ���ο� ����
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (roadGrid[x, y] && !visited[x, y])
                {
                    // ���� ����� �ܰ� ���η� ���� ��� ����
                    ConnectToNearestRoad(x, y);
                }
            }
        }
    }

    void ConnectToNearestRoad(int startX, int startY)
    {
        // �����ϰ� ����/���� ��η� ����
        int nearestX = startX < width / 2 ? 0 : width - 1; // �¿� �ܰ� �� ����� ��
        for (int x = Mathf.Min(startX, nearestX); x <= Mathf.Max(startX, nearestX); x++)
        {
            roadGrid[x, startY] = true;
        }
        // ����� �ܰ� ���ο��� ���������� ���� ��� �߰�
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