using UnityEngine;
using Gley.TrafficSystem;

public class RoadRuntimeToggle_M : MonoBehaviour
{
    // ── 도로 끌 때 ──────────────────────────
    public void DisableRoad(GameObject road)
    {
        Collider col = road.GetComponent<Collider>();
        if (col == null) return;

        Bounds b = col.bounds;
        float r = Mathf.Max(b.extents.x, b.extents.z);

        // v3.1.1 → 2-인수 버전
        API.DisableAreaWaypoints(b.center, r);
        API.ClearTrafficOnArea(b.center, r);

        road.SetActive(false);
    }

    // ── 도로 켤 때 ──────────────────────────
    public void EnableRoad(GameObject road)
    {
        road.SetActive(true);

        Collider col = road.GetComponent<Collider>();
        if (col == null) return;

        Bounds b = col.bounds;
        float r = Mathf.Max(b.extents.x, b.extents.z);

        // 웨이포인트 재활성 : 존재하지 않으면 삭제
        //API.EnableAreaWaypoints(b.center, r);
    }
}
