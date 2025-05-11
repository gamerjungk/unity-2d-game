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
            Debug.LogError("Player �Ǵ� Main Camera�� ������� �ʾҽ��ϴ�!");
            return;
        }
        mainCamera.depth = 0;
    }

    void LateUpdate()
    {
        Vector3 targetPosition = player.position;
        targetPosition.z = mainCamera.transform.position.z; // Z�� -10 ����
        mainCamera.transform.position = targetPosition;
    }
}