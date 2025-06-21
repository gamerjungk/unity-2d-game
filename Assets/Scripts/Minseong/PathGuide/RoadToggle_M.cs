using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Gley.TrafficSystem;

[RequireComponent(typeof(NavMeshModifier))] // 해당 cs가 붙은 오브젝트에 NavMeshModifier가 반드시 필요
public class RoadToggle : MonoBehaviour
{
    [Header("Road Toggle 옵션")] // 헤더 표시
    [Tooltip("교차로·고정도로 같이 항상 Walkable 로 유지할 블록은 체크하세요")]
    [SerializeField] public bool alwaysWalkable = false; // 항상 보행 가능 영역으로 유지할지 여부

    private NavMeshModifier mod; // NavMeshModifier 컴포넌트 캐시
    private Renderer[] renderers; // 자식 렌더러 배열 캐시
    private Transform[] roadNodes; // 자식 Transform 배열 캐시 (RoadNode 포함)

    void Awake()
    {
        mod = GetComponent<NavMeshModifier>(); // NavMeshModifier 컴포넌트 가져오기
        mod.overrideArea = true; // 에디터 설정 무시하고 cs에서 영역 지정 허용

        renderers = GetComponentsInChildren<Renderer>(true); // 비활성화된 자식 포함 모든 렌더러 가져오기
        roadNodes = GetComponentsInChildren<Transform>(true); // 비활성화된 자식 포함 모든 Transform 가져오기

        // 상시 도로는 처음부터 Walkable(0) 로 고정
        if (alwaysWalkable)
            mod.area = 0;   // Walkable
    }

    /// 가변 도로의 Walkable ↔︎ NotWalkable 토글
    /// alwaysWalkable=true 블록에서는 호출을 무시함
    public void SetActiveRoad(bool isActive)
    {
        if (alwaysWalkable) return;   // 교차로 등 예외

        /* --- 기존 기능 유지 ---- */
        mod.area = isActive ? 0 : 1; // NavMesh 영역

        // 렌더러 활성/비활성화로 시각적 표시 토글
        foreach (var rend in renderers) rend.enabled = isActive;

        // 모든 자식 Transform 순회
        foreach (var tf in roadNodes)
            if (tf.CompareTag("RoadNode")) // 태그가 "RoadNode"인 경우에만
                tf.gameObject.SetActive(isActive); // 게임오브젝트 활성/비활성화

        /* 웨이포인트·차량 제어 : 끄는 경우에만 실행 */
        if (!isActive)
        {
            Collider col = GetComponent<Collider>(); // 충돌 영역 검사용 Collider 가져오기
            if (col == null) return; // Collider가 없으면 종료

            Bounds b = col.bounds; // Collider의 경계 영역 계산
            float rad = Mathf.Max(b.extents.x, b.extents.z); // 반경 계산 (X, Z 중 큰 값)

            API.DisableAreaWaypoints(b.center, rad); // 해당 영역 내 웨이포인트 비활성화
            API.ClearTrafficOnArea(b.center, rad); // 해당 영역 내 주행 중인 차량 회수/삭제
        }
    }
}
