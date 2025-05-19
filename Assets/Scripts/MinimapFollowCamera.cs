using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MinimapFollowCamera : MonoBehaviour
{
    public Transform target;             // ����ٴ� �÷��̾�
    public Camera minimapCamera;         // �� ��ũ��Ʈ�� ����� �̴ϸ� ī�޶�
    public Camera mainCamera;            // ���� ī�޶� ���� (�þ� �����)
    public float zoomOutFactor = 3f;   // ����ī�޶󺸴� �󸶳� �� �а� ����

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 newPos = target.position;
        newPos.y = transform.position.y; // ������ ���� ����
        transform.position = newPos;

        transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
    }
}
