using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshModifier))]
public class RoadToggle : MonoBehaviour
{
    [Header("Road Toggle 옵션")]
    [Tooltip("교차로·고정도로 같이 항상 Walkable 로 유지할 블록은 체크하세요")]
    [SerializeField] private bool alwaysWalkable = false;

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
    public void SetActiveRoad(bool enable)
    {
        if (alwaysWalkable) return;           // 교차로 등은 건드리지 않음

        // 0 = Walkable, 1 = Not Walkable  (Navigation ▶ Areas 창 순서 기준)
        mod.area = enable ? 0 : 1;

        // 시각적 토글 (원하지 않으면 주석 처리)
        foreach (var r in renderers) r.enabled = enable;

        // RoadNode 도 같이 켜고/끄기
        foreach (var tf in roadNodes)
            if (tf.CompareTag("RoadNode"))
                tf.gameObject.SetActive(enable);
    }
}
