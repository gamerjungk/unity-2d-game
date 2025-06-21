using UnityEngine;

public class TopDownCamera_M : MonoBehaviour
{
    public Transform target; // �ڵ���
    public Vector3 offset = new Vector3(0, 10, 0); // ������ �Ʒ��� ����

    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        // �÷��̾� ������ offset��ŭ ��ġ
        Vector3 targetPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        // �׻� ������(90��)���� ����
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