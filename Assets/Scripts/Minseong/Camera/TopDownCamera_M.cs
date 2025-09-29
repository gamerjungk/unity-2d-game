using UnityEngine;

public class TopDownCamera_M : MonoBehaviour
{
   public Transform target; // 타겟(자동차)의 Transform 컴포넌트 할당
    public Vector3 offset = new Vector3(0, 10, 0); // 위에서 아래로 보기

    public float followSpeed = 5f; // 카메라 속도(5f) 설정

    void LateUpdate()
    {
        if (target == null) return; // 대상이 할당되지 않으면 종료

        // 플레이어 위에서 offset만큼 위치
        Vector3 targetPos = target.position + offset;
        // 현 위치에서 목표 위치로 followSpeed 비율만큼 이동
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        // 항상 정수직(90도)으로 고정
        transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
    }
    
}

/*
 탑 다운 카메라 스크립트
 - target: 따라갈 대상 (플레이어 등)
    - offset으로 카메라 위치 조정
    - followSpeed로 설정 및 Inspector에서 조정 가능
    - LateUpdate()에서 카메라 위치와 회전 업데이트
*/