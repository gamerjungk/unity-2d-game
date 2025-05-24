using UnityEngine;

public class IntersectionPoolManager : MonoBehaviour
{
    [Header("Random Road Settings")]
    public int minActiveBlocks = 8;   // 최소 활성 도로 수
    public int totalBlocks = 12; // Vertical / Horizontal 블록 개수

    private void Start()
    {
        RandomizeRoads();
    }

    /// <summary>
    /// 세로 · 가로 블록을 무작위로 ON/OFF.
    /// 중심 블록(6,7번)은 항상 ON.
    /// </summary>
    public void RandomizeRoads()
    {
        int activeCount = 0;

        for (int i = 1; i <= totalBlocks; i++)
        {
            bool forceActive = (i == 6 || i == 7);            // 중심 도로
            bool verticalActive = forceActive || Random.value > 0.5f;
            bool horizontalActive = forceActive || Random.value > 0.5f;

            if (verticalActive) activeCount++;
            if (horizontalActive) activeCount++;

            SetBlockActive($"Vertical_block{i}", verticalActive);
            SetBlockActive($"Horizontal_block{i}", horizontalActive);
        }

        Debug.Log($"✅ 도로 생성 완료: 활성 블록 수 = {activeCount}");

        if (activeCount < minActiveBlocks)
            Debug.LogWarning("⚠️ 활성 도로 수가 너무 적습니다. 경로가 불안정할 수 있습니다.");
    }

    /// <summary>
    /// 이름으로 도로 블록 ON/OFF + NavMesh 즉시 갱신
    /// </summary>
    public void SetBlockActive(string name, bool isActive)
    {
        GameObject obj = GameObject.Find(name);
        if (obj == null)
        {
            Debug.LogWarning($"🚧 도로 오브젝트 {name}을(를) 찾을 수 없습니다.");
            return;
        }

        // RoadToggle 있으면 Area 변경 + 노드 동기화
        var toggle = obj.GetComponent<RoadToggle>();
        if (toggle != null)
            toggle.SetActiveRoad(isActive);
        else
            obj.SetActive(isActive); // 예외적으로 Toggle 없는 경우

        // NavMeshSurface 실시간 재빌드
        DestinationManager.Instance?.RebuildNavMesh();
    }
}
