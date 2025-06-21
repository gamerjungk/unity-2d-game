using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove_M : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 100f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Ű���� �Է¸� ó��
        float move = Input.GetAxis("Vertical") * moveSpeed;
        float turn = Input.GetAxis("Horizontal") * turnSpeed;

        // ���� �̵�
        rb.MovePosition(rb.position + transform.forward * move * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn * Time.fixedDeltaTime, 0f));
    }
}

/*
    플레이어 이동 스크립트
    - Rigidbody를 사용하여 물리 기반 이동 구현
    - moveSpeed와 turnSpeed로 이동 및 회전 속도 조정
    - FixedUpdate()에서 입력에 따라 이동 및 회전 처리
    - 입력은 Input.GetAxis사용
*/