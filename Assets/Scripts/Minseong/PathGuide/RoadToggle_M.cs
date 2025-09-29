using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Gley.TrafficSystem;

[RequireComponent(typeof(NavMeshModifier))]
public class RoadToggle : MonoBehaviour
{
    [Header("Road Toggle 옵션")]
    [Tooltip("교차로·고정도로 같이 항상 Walkable 로 유지할 블록은 체크하세요")]
    [SerializeField] public bool alwaysWalkable = true;

    private NavMeshModifier mod;
    private Renderer[] renderers;
    private Transform[] roadNodes;

    void Awake()
    {
        mod = GetComponent<NavMeshModifier>();
        mod.overrideArea = true;

        renderers = GetComponentsInChildren<Renderer>(true);
        roadNodes = GetComponentsInChildren<Transform>(true);

        // 상시 도로는 처음부터 Walkable(0)로 고정
        if (alwaysWalkable)
            mod.area = 0;
    }

    /// 가변 도로의 Walkable ↔︎ NotWalkable 토글
    /// (테스트용: 항상 Walkable 유지)
    public void SetActiveRoad(bool isActive)
    {
        if (alwaysWalkable) return;

        // 항상 Walkable 유지 (빨간 웨이포인트 안 생기게)
        mod.area = 0;

        // 렌더러 활성/비활성화로 시각적 표시만 바꿔줌
        foreach (var rend in renderers)
            rend.enabled = isActive;

        // RoadNode 오브젝트도 켜고 끄기
        foreach (var tf in roadNodes)
            if (tf.CompareTag("RoadNode"))
                tf.gameObject.SetActive(isActive);
        
        if (!isActive)
        {
            Collider col = GetComponent<Collider>();
            if (col == null) return;

            Bounds b = col.bounds;
            float rad = Mathf.Max(b.extents.x, b.extents.z);

            //API.DisableAreaWaypoints(b.center, rad);
            //API.ClearTrafficOnArea(b.center, rad);
        }
        
    }
    // 추가: 에디터/런타임 어디서든 Walkable을 강제
    void OnEnable()
    {
        if (mod == null) mod = GetComponent<NavMeshModifier>();
        mod.overrideArea = true;
        if (mod.area != 0) mod.area = 0;
    }

    void OnValidate()
    {
        if (mod == null) mod = GetComponent<NavMeshModifier>();
        if (mod != null)
        {
            mod.overrideArea = true;
            if (mod.area != 0) mod.area = 0;
        }
    }

}
