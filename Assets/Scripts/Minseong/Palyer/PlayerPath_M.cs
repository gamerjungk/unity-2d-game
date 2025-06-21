using UnityEngine;

public class PlayerPath : MonoBehaviour
{
    [SerializeField] private float arriveRadius = 1.2f;

    private void Update()
    {
        var dm = DestinationManager.Instance;
        if (dm == null || dm.CurrentTarget == null) return;

        if (Vector3.Distance(transform.position, dm.CurrentTarget.position) <= arriveRadius)
        {
            dm.ArrivedCurrentTarget();
        }
    }
}

/*
 플레이어가 목적지에 도달했는지 확인하는 스크립트
목표 지점에 도달했는지 확인하고, 도달했을 경우 DestinationManager의 ArrivedCurrentTarget 메서드를 호출
*/
