using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MiniMapFollow : MonoBehaviour
{
    public Transform target; // 타겟의 Transform 컴포넌트 핼당

    void LateUpdate()
    {
        // 대상이 할당되어 있으면
        if (target != null)
        {
            Vector3 newPosition = target.position; // 대상의 현재 위치를 기반으로 새 위치 계산
            newPosition.y = transform.position.y; // Y축(높이)은 고정하여 미니맵 높이 유지
            transform.position = newPosition; // 계산된 위치로 카메라 이동

            // 방향도 동일하게 맞춤 (Y축 회전만 반영)
            transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
        }
    }
}

/*
 미니맵이 플레이어를 따라가도록 설정 스크립트
 - 플레이어의 위치를 따라가며 Y축 회전만 적용
 - LateUpdate()에서 target의 위치를 따라가고 Y축 회전만 적용
 - target은 플레이어의 Transform으로 설정
*/