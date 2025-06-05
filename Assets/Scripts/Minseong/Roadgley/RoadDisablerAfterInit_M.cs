using System.Collections;
using UnityEngine;
using Gley.TrafficSystem;

public class RoadDisablerAfterInit_M : MonoBehaviour
{
    [SerializeField] Transform roadRoot;       // RoadRoot를 Inspector에 드래그
    [Range(0f, 1f)] public float disableRatio = 0.30f;   // 30 % 차단

    IEnumerator Start()
    {
        // 초기화 완료 대기
        yield return new WaitUntil(() => FindObjectOfType<TrafficComponent>() != null);

        // 모든 RoadToggle 탐색
        var toggles = roadRoot.GetComponentsInChildren<RoadToggle>(true);
        Debug.Log($"[Disabler] found {toggles.Length} toggles");

        // 차단 실행
        foreach (var tog in toggles)
        {
            // (1) 외곽도로·고정도로 건너뛰기
            if (tog.alwaysWalkable)                 // Inspector 체크박스
                continue;                           // 또는  tog.CompareTag("FixedRoad")

            // (2) disableRatio% 만큼 무작위로 비활성
            if (Random.value < disableRatio)
            {
                Debug.Log($"[Disabler] disable {tog.name}");
                tog.SetActiveRoad(false);
            }
        }
    }
}
