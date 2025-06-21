using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Gley.TrafficSystem;

[RequireComponent(typeof(NavMeshModifier))]
public class RoadToggle : MonoBehaviour
{
    [Header("Road Toggle 옵션")]
    [Tooltip("교차로·고정도로 같이 항상 Walkable 로 유지할 블록은 체크하세요")]
    [SerializeField] public bool alwaysWalkable = false;

    private NavMeshModifier mod;
    private Renderer[] renderers;
    private Transform[] roadNodes;

    void Awake()
    {
        mod = GetComponent<NavMeshModifier>();
        mod.overrideArea = true;

        renderers = GetComponentsInChildren<Renderer>(true);
        roadNodes = GetComponentsInChildren<Transform>(true);

        // 상시 도로는 처음부터 Walkable(0) 로 고정
        if (alwaysWalkable)
            mod.area = 0;   // Walkable
    }

    /// <summary>
    /// 가변 도로의 Walkable ↔︎ NotWalkable 토글
    /// alwaysWalkable=true 블록에서는 호출을 무시합니다.
    /// </summary>
    public void SetActiveRoad(bool isActive)
    {
        if (alwaysWalkable) return;   // 교차로 등 예외

        /* --- 기존 기능 유지 ---- */
        mod.area = isActive ? 0 : 1;                      // NavMesh 영역
        foreach (var rend in renderers) rend.enabled = isActive;
        foreach (var tf in roadNodes)
            if (tf.CompareTag("RoadNode"))
                tf.gameObject.SetActive(isActive);

        /* 웨이포인트·차량 제어 : 끄는 경우에만 실행 */
        if (!isActive)
        {
            Collider col = GetComponent<Collider>();
            if (col == null) return;

            Bounds b = col.bounds;
            float rad = Mathf.Max(b.extents.x, b.extents.z);

            API.DisableAreaWaypoints(b.center, rad);   // 웨이포인트 OFF
            API.ClearTrafficOnArea(b.center, rad);     // 이미 달리던 차 회수
        }
    }
}

/* 
 NavMeshModifier 를 사용하여 도로 블록의 Walkable 상태를 토글하는 스크립트
    alwaysWalkable 옵션이 true인 경우, 해당 블록은 항상 Walkable 상태로 유지
    SetActiveRoad 메서드를 통해 가변 도로의 활성화/비활성화를 제어
    비활성화 시, 해당 영역의 웨이포인트와 차량을 회수
    Gley Traffic System과 통합되어 차량 제어를 지원
*/