using System.Collections;
using UnityEngine;
using Gley.TrafficSystem;
using Unity.AI.Navigation;            // NavMeshModifier

public class RoadDisablerAfterInit_M : MonoBehaviour
{
    [SerializeField] Transform roadRoot;       // RoadRoot
    [Tooltip("IntersectionPoolManager 가 끝난 뒤 몇 프레임 기다릴지")]
    [SerializeField] int waitFrames = 2;

    IEnumerator Start()
    {
        // ① Traffic System 초기화 완료 대기
        yield return new WaitUntil(() =>
            Object.FindAnyObjectByType<TrafficComponent>() != null);

        // ② IntersectionPoolManager 가 NavMesh Rebuild 를 끝낼 때까지
        //    한두 프레임 더 대기 (실행 순서 불확정 대비)
        for (int i = 0; i < waitFrames; ++i)
            yield return null;

        int effected = 0;

        // ③ RoadRoot 아래 모든 RoadToggle 검사
        foreach (var tog in roadRoot.GetComponentsInChildren<RoadToggle>(true))
        {
            // (A) 외곽‧항상 켜둘 도로 건너뜀
            if (tog.alwaysWalkable) continue;

            bool visuallyOff = !tog.gameObject.activeInHierarchy ||
                               !AnyRendererEnabled(tog.transform);

            bool areaBlocked = false;
            var mod = tog.GetComponent<NavMeshModifier>();
            if (mod && mod.overrideArea && mod.area == 1)      // 1 = Not Walkable
                areaBlocked = true;

            // IntersectionPoolManager 에 의해 이미 비활성화된 블록만 처리
            if (visuallyOff || areaBlocked)
            {
                DisableWaypointsAndTraffic(tog);
                effected++;
            }
        }

        Debug.Log($"[AfterInit] disabled waypoint 영역 수 = {effected}");
    }

    // ----------------- Helper -----------------
    bool AnyRendererEnabled(Transform root)
    {
        foreach (var r in root.GetComponentsInChildren<Renderer>(true))
            if (r.enabled) return true;
        return false;
    }

    void DisableWaypointsAndTraffic(RoadToggle tog)
    {
        Collider col = tog.GetComponentInChildren<Collider>(true);
        if (col == null) return;

        Bounds b = col.bounds;
        float r = Mathf.Max(b.extents.x, b.extents.z);

        API.DisableAreaWaypoints(b.center, r);  // 웨이포인트 OFF
        API.ClearTrafficOnArea(b.center, r);         // 이미 달리는 차량 회수
    }
}
