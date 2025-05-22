using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MinimapFollowCamera : MonoBehaviour
{
    public Transform target;             // 따라다닐 플레이어
    public Camera minimapCamera;         // 이 스크립트에 연결된 미니맵 카메라
    public Camera mainCamera;            // 메인 카메라 참조 (시야 참고용)
    public float zoomOutFactor = 3f;   // 메인카메라보다 얼마나 더 넓게 볼지

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 newPos = target.position;
        newPos.y = transform.position.y; // 고정된 높이 유지
        transform.position = newPos;

        transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
    }
}
