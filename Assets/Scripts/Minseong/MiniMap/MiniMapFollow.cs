using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MiniMapFollow : MonoBehaviour
{
    public Transform target;

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 newPosition = target.position;
            newPosition.y = transform.position.y; // 높이 고정
            transform.position = newPosition;

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