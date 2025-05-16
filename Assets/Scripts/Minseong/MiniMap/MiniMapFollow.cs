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
            newPosition.y = transform.position.y; // ���� ����
            transform.position = newPosition;

            // ���⵵ �����ϰ� ���� (Y�� ȸ���� �ݿ�)
            transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
        }
    }
}
