using System.Collections;
using UnityEngine;
using Gley.TrafficSystem;
using Unity.AI.Navigation;
using UnityEditor.Rendering;

public class RoadDisablerAfterInit_M : MonoBehaviour
{
    [SerializeField] Transform roadRoot; // 검사할 도로 블록들의 루트 Transform
    [Tooltip("IntersectionPoolManager 가 끝난 뒤 몇 프레임 기다릴지")]
    [SerializeField] int waitFrames = 2; // NavMesh 재빌드 후 대기할 프레임 수

    // 게임 시작 시 자동 실행되는 코루틴
    IEnumerator Start()
    {
        // ① Traffic System 초기화 완료 대기
        yield return new WaitUntil(() =>
            Object.FindAnyObjectByType<TrafficComponent>() != null);

        // ② IntersectionPoolManager 가 NavMesh Rebuild 를 끝낼 때까지
        //    한두 프레임 더 대기 (실행 순서 불확정 대비)
        for (int i = 0; i < waitFrames; ++i)
            yield return null; // 매 프레임마다 대기

        int effected = 0; // 처리된 블록 수 카운터 초기화

        // ③ RoadRoot 아래 모든 RoadToggle 검사
        foreach (var tog in roadRoot.GetComponentsInChildren<RoadToggle>(true))
        {
            // (A) 외곽‧항상 켜둘 도로 건너뜀
            if (tog.alwaysWalkable) continue;

            // (B) 시각적으로 꺼져 있는지 확인
            bool visuallyOff = !tog.gameObject.activeInHierarchy ||
                               !AnyRendererEnabled(tog.transform);

            // (C) NavMeshModifier가 NotWalkable(1) 영역인지 확인
            bool areaBlocked = false;
            var mod = tog.GetComponent<NavMeshModifier>();
            if (mod && mod.overrideArea && mod.area == 1) // overrideArea가 켜져 있고 area가 1이면 true
                areaBlocked = true;

            // IntersectionPoolManager 에 의해 이미 비활성화된 블록만 처리
            if (visuallyOff || areaBlocked)
            {
                DisableWaypointsAndTraffic(tog); // 웨이포인트 및 차량 차단
                effected++; // 카운터 증가
            }
        }

        Debug.Log($"[AfterInit] disabled waypoint 영역 수 = {effected}");
    }

    // ----------------- Helper -----------------
    // 자식 렌더러 중 활성화된 것이 있는지 확인
    bool AnyRendererEnabled(Transform root)
    {
        // 하나라도 활성화된 렌더러가 있으면 true 반환
        foreach (var r in root.GetComponentsInChildren<Renderer>(true))
            if (r.enabled) return true;
        return false; // 모두 비활성화 상태면 false 반환
    }

    // 블록 차단 처리 메서드
    void DisableWaypointsAndTraffic(RoadToggle tog)
    {
        // 자식에서 콜라이더를 찾아 영역 중심·반경 계산
        Collider col = tog.GetComponentInChildren<Collider>(true);
        if (col == null) return; // 콜라이더 없으면 중단

        // 콜라이더 경계 상자 가져오기
        Bounds b = col.bounds;
        // X/Z 중 큰 extents를 반경으로 사용
        float r = Mathf.Max(b.extents.x, b.extents.z);

        API.DisableAreaWaypoints(b.center, r); // 웨이포인트 OFF
        API.ClearTrafficOnArea(b.center, r); // 이미 달리는 차량 회수
    }
}
