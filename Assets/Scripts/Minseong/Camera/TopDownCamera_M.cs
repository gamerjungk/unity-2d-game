using UnityEngine;

public class TopDownCamera_M : MonoBehaviour
{
    public Transform target; // 자동차
    public Vector3 offset = new Vector3(0, 10, 0); // 위에서 아래로 보기

    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        // 플레이어 위에서 offset만큼 위치
        Vector3 targetPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        // 항상 정수직(90도)으로 고정
        transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
    }
}
