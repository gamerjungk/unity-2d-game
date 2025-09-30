using System.Collections;
using UnityEngine;
using Gley.TrafficSystem;
using Unity.AI.Navigation;

public class RoadBlocker_M : MonoBehaviour
{
    [Header("필수: RoadRoot 드래그")] // 헤더 표시
    [SerializeField] Transform roadRoot; // 검사할 도로 블록들의 루트 Transform

    [Tooltip("IntersectionPoolManager 가 NavMesh를 다시 빌드한 뒤 대기할 프레임 수")]
    [SerializeField] int delayFrames = 5; // NavMesh 재빌드 후 대기할 프레임 수

    // 게임 시작 시 자동 실행되는 코루틴
    IEnumerator Start()
    {
        //------------------------------------------------------------------
        // 1) Traffic System 이 완전히 초기화될 때까지 대기
        //------------------------------------------------------------------
        TrafficComponent tc = null; // TrafficComponent 참조 저장 변수
        // 조건이 참이 될 때까지 대기
        yield return new WaitUntil(() =>
        {
            // 씬에서 TrafficComponent 찾기
            tc = Object.FindAnyObjectByType<TrafficComponent>();
            return tc != null; // 찾았으면 true 반환
        });

        //------------------------------------------------------------------
        // 2) IntersectionPoolManager 가 NavMesh Rebuild 를 끝내도록
        //    지정한 프레임수만큼 여유를 둔다
        //------------------------------------------------------------------
        for (int i = 0; i < delayFrames; ++i) // delayFrames 프레이만큼 매 프레임 대기
            yield return null;

        int blocked = 0; // 차단된 블록 개수 카운터

        //------------------------------------------------------------------
        // 3) RoadRoot 아래 RoadToggle 전부 검사
        //------------------------------------------------------------------
        foreach (var tog in roadRoot.GetComponentsInChildren<RoadToggle>(true))
        {
            // 외곽·고정 도로 건너뜀
            if (tog == null || tog.alwaysWalkable)
                continue;

            // ── (A) 렌더러가 이미 꺼졌는가?
            bool visuallyOff = !tog.gameObject.activeInHierarchy || // 게임오브젝트 비활성화
                               !AnyRendererEnabled(tog.transform); // 자식 렌더러 모두 비활성화

            // ── (B) NavMeshModifier 로 Area 가 “Not Walkable(1)” 인가?
            bool areaOff = false;
            var mod = tog.GetComponent<NavMeshModifier>(); // NavMeshModifier 컴포넌트 가져오기
            if (mod && mod.overrideArea && mod.area == 1) // overrideArea가 켜져 있고 영역이 1이면
                areaOff = true; // 보행 불가 영역으로 간주

            // 둘 중 하나라도 참이면 “꺼진 블록”으로 판단
            if (visuallyOff || areaOff)
            {
                // 해당 영역 웨이포인트 및 차량 차단
                DisableZone(tog);
                blocked++; // 차단 카운트 증가
            }
        }

        Debug.Log($"[RoadBlocker] waypoint 차단 블록 수 = {blocked}");
    }

    // --- Helper ---
    // 자식 렌더러 중 활성화된 것이 있는지 확인
    bool AnyRendererEnabled(Transform root)
    {
        foreach (var r in root.GetComponentsInChildren<Renderer>(true))
            if (r.enabled) // 하나라도 enabled면 true 반환
                return true;
        return false; // 모두 비활성화면 false 반환
    }

    // 특정 RoadToggle 영역 차단 처리
    void DisableZone(RoadToggle tog)
    {
        // 가장 큰 콜라이더를 찾아서 영역 중심·반경 계산
        Collider col = tog.GetComponentInChildren<Collider>(true);
        if (col == null) return; // Collider 없으면 종

        // 경계 상자 가져오기
        Bounds b = col.bounds;
        float radius = Mathf.Max(b.extents.x, b.extents.z); // X/Z 중 큰 extents를 반경으로 사용

        // 1) 해당 영역 웨이포인트 OFF
        API.DisableAreaWaypoints(b.center, radius);

        // 2) 이미 달리고 있던 차량 회수
        API.ClearTrafficOnArea(b.center, radius);
    }
}
