using UnityEngine;

// cs가 붙은 오브젝트에 Rigidbody 컴포넌트를 자동으로 추가하도록 요구
[RequireComponent(typeof(Rigidbody))]
public class PlayerMove_M : MonoBehaviour
{
    public float moveSpeed = 10f; // 앞뒤 이동 속도 설정 값
    public float turnSpeed = 100f; // 좌우 회전 속도 설정 값

    private Rigidbody rb; // 물리 기반 이동을 처리할 Rigidbody 참조 변수

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // 시작 시 Rigidbody 컴포넌트를 찾아서 rb에 할당
    }

    void FixedUpdate()
    {
        // 키보드 입력만 처리
        float move = Input.GetAxis("Vertical") * moveSpeed;
        float turn = Input.GetAxis("Horizontal") * turnSpeed;

        // 실제 이동
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