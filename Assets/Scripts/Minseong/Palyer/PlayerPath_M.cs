using UnityEngine;

public class PlayerPath : MonoBehaviour
{
    // 도착 판정 반지름 설정 (Inspector에서 조정 가능)
    [SerializeField] private float arriveRadius = 1.2f;

    private void Update()
    {
        // DestinationManager 싱글턴 인스턴스 가져오기
        var dm = DestinationManager.Instance;
        // DestinationManager 또는 CurrentTarget이 없으면 로직 중단
        if (dm == null || dm.CurrentTarget == null) return;

        // 플레이어 위치(transform.position)와 현재 목표 위치 사이의 거리를 계산
        // 거리가 arriveRadius 이하이면 도착 처리 호출
        if (Vector3.Distance(transform.position, dm.CurrentTarget.position) <= arriveRadius)
        {
            dm.ArrivedCurrentTarget(); // 목표 도착 시 DestinationManager에 알림
        }
    }
}
