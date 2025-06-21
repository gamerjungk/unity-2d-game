using UnityEngine;
using Gley.TrafficSystem;

public class RoadRuntimeToggle_M : MonoBehaviour
{
    // ── 도로 끌 때
    public void DisableRoad(GameObject road)
    {
        // 전달된 도로 오브젝트에서 Collider 컴포넌트 가져오기  
        Collider col = road.GetComponent<Collider>();
        if (col == null) return; // Collider가 없으면 더 이상 진행하지 않음  

        // Collider의 경계 상자를 가져와서  
        Bounds b = col.bounds;
        // X와 Z 확장값 중 큰 값을 반경으로 사용  
        float r = Mathf.Max(b.extents.x, b.extents.z);

        // 해당 영역 내 웨이포인트 비활성화 (v3.1.1 두 인수 버전 API)  
        API.DisableAreaWaypoints(b.center, r);
        // 해당 영역 내 이동 중인 차량 회수  
        API.ClearTrafficOnArea(b.center, r);

        // 도로 게임오브젝트 비활성화(보이지 않게 함)  
        road.SetActive(false);
    }

    // ── 도로 켤 때
    public void EnableRoad(GameObject road)
    {
        // 도로 게임오브젝트 활성화(다시 보이게 함)  
        road.SetActive(true);

        // 활성화된 도로에서 Collider 컴포넌트 가져오기  
        Collider col = road.GetComponent<Collider>();
        if (col == null) return;               // Collider가 없으면 더 이상 진행하지 않음  

        // Collider의 경계 상자를 가져와서  
        Bounds b = col.bounds;
        // X와 Z 확장값 중 큰 값을 반경으로 사용  
        float r = Mathf.Max(b.extents.x, b.extents.z);

        // 웨이포인트 재활성화: 필요 시 API 호출  
        // API.EnableAreaWaypoints(b.center, r);  
    }
}
