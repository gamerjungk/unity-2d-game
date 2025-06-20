using System.Collections;
using UnityEngine;
using Gley.TrafficSystem;
using Unity.AI.Navigation;   // NavMeshModifier

public class RoadBlocker_M : MonoBehaviour
{
    [Header("필수: RoadRoot 드래그")]
    [SerializeField] Transform roadRoot;

    [Tooltip("IntersectionPoolManager 가 NavMesh를 다시 빌드한 뒤 대기할 프레임 수")]
    [SerializeField] int delayFrames = 5;

    IEnumerator Start()
    {
        //------------------------------------------------------------------
        // 1) Traffic System 이 완전히 초기화될 때까지 대기
        //------------------------------------------------------------------
        TrafficComponent tc = null;
        yield return new WaitUntil(() =>
        {
            tc = Object.FindAnyObjectByType<TrafficComponent>();
            return tc != null;
        });

        //------------------------------------------------------------------
        // 2) IntersectionPoolManager 가 NavMesh Rebuild 를 끝내도록
        //    지정한 프레임수만큼 여유를 둔다
        //------------------------------------------------------------------
        for (int i = 0; i < delayFrames; ++i)
            yield return null;

        int blocked = 0;

        //------------------------------------------------------------------
        // 3) RoadRoot 아래 RoadToggle 전부 검사
        //------------------------------------------------------------------
        foreach (var tog in roadRoot.GetComponentsInChildren<RoadToggle>(true))
        {
            if (tog == null || tog.alwaysWalkable)
                continue;                       // 외곽·고정 도로 건너뜀

            // ── (A) 렌더러가 이미 꺼졌는가?
            bool visuallyOff = !tog.gameObject.activeInHierarchy ||
                               !AnyRendererEnabled(tog.transform);

            // ── (B) NavMeshModifier 로 Area 가 “Not Walkable(1)” 인가?
            bool areaOff = false;
            var mod = tog.GetComponent<NavMeshModifier>();
            if (mod && mod.overrideArea && mod.area == 1)
                areaOff = true;

            // 둘 중 하나라도 참이면 “꺼진 블록”으로 판단
            if (visuallyOff || areaOff)
            {
                DisableZone(tog);
                blocked++;
            }
        }

        Debug.Log($"[RoadBlocker] waypoint 차단 블록 수 = {blocked}");
    }

    // ───────────────────────────────────────────── Helper ──
    bool AnyRendererEnabled(Transform root)
    {
        foreach (var r in root.GetComponentsInChildren<Renderer>(true))
            if (r.enabled)
                return true;
        return false;
    }

    void DisableZone(RoadToggle tog)
    {
        // 가장 큰 콜라이더를 찾아서 영역 중심·반경 계산
        Collider col = tog.GetComponentInChildren<Collider>(true);
        if (col == null) return;

        Bounds b = col.bounds;
        float radius = Mathf.Max(b.extents.x, b.extents.z);

        // 1) 해당 영역 웨이포인트 OFF
        API.DisableAreaWaypoints(b.center, radius);

        // 2) 이미 달리고 있던 차량 회수
        API.ClearTrafficOnArea(b.center, radius);
    }
}
