using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraSetup : MonoBehaviour
{
    public Tilemap tilemap;
    public Camera minimapCamera;

    void Start()
    {
        if (tilemap == null || minimapCamera == null)
        {
            Debug.LogError("Tilemap 또는 MinimapCamera가 연결되지 않았습니다!");
            return;
        }
        minimapCamera.depth = 1;
        AdjustCameraToTilemap();
    }

    void AdjustCameraToTilemap()
    {
        BoundsInt bounds = tilemap.cellBounds;
        Vector3 center = bounds.center;
        center.z = minimapCamera.transform.position.z;
        minimapCamera.transform.position = center;

        float tilemapHeight = bounds.size.y;
        float tilemapWidth = bounds.size.x;
        float orthoSize = tilemapHeight / 2f;

        float screenAspect = (float)Screen.width / Screen.height;
        float tilemapAspect = tilemapWidth / tilemapHeight;
        if (tilemapAspect > screenAspect)
        {
            orthoSize = tilemapWidth / (2f * screenAspect);
        }

        minimapCamera.orthographicSize = orthoSize;
    }
}