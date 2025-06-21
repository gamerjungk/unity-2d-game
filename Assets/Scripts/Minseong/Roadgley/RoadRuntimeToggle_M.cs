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

/*
    - Gley Traffic System API를 사용하여 웨이포인트와 차량을 관리
    - DisableRoad 메서드는 도로를 비활성화하고 해당 영역의 웨이포인트와 차량을 회수
    - EnableRoad 메서드는 도로를 활성화하지만 웨이포인트는 활성화하지 않음 (주석 처리된 부분)
*/