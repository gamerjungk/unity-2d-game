using UnityEngine;

public class PlayerFollowCamera : MonoBehaviour
{
    public Transform player;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        if (player == null || mainCamera == null)
        {
            Debug.LogError("Player 또는 Main Camera가 연결되지 않았습니다!");
            return;
        }
        mainCamera.depth = 0;
    }

    void LateUpdate()
    {
        Vector3 targetPosition = player.position;
        targetPosition.z = mainCamera.transform.position.z; // Z는 -10 유지
        mainCamera.transform.position = targetPosition;
    }
}