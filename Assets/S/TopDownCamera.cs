using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    public Transform target;           // 자동차
    public Vector3 offset = new Vector3(0, 5, -5);
    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + offset;
        transform.position = targetPos;

        // 자동차가 바라보는 방향으로 회전 (수직 시점이므로 x/z는 고정하고 y축만 따라감)
        Quaternion desiredRotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, followSpeed * Time.deltaTime);
    }
}
