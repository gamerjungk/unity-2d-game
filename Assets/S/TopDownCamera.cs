using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    public Transform target;           // �ڵ���
    public Vector3 offset = new Vector3(0, 5, -5);
    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + offset;
        transform.position = targetPos;

        // �ڵ����� �ٶ󺸴� �������� ȸ�� (���� �����̹Ƿ� x/z�� �����ϰ� y�ุ ����)
        Quaternion desiredRotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, followSpeed * Time.deltaTime);
    }
}
