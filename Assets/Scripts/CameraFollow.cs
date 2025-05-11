using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;        // 카메라가 따라갈 플레이어 (차량)
    public float smoothSpeed = 0.125f; // 카메라가 따라가는 부드러움 정도
    public Vector3 offset;          // 카메라와 플레이어 간의 위치 차이

    void Start()
    {
        // 카메라 시작 위치 설정
        if (player != null)
        {
            transform.position = player.position + offset;
        }
    }

    void LateUpdate()
    {
        // 플레이어의 위치만 따라가도록 설정
        if (player != null)
        {
            // 플레이어 위치에 맞춰 카메라 위치를 부드럽게 이동
            Vector3 desiredPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            // 카메라 회전하지 않도록 설정 (고정된 각도 유지)
            transform.rotation = Quaternion.Euler(30f, 0f, 0f); // 예시: 카메라가 일정한 각도로 고정됨
        }
    }
}