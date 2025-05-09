using UnityEngine;

public class CarController : MonoBehaviour
{
    public float moveSpeed = 5f;        // 자동차의 이동 속도
    public float turnSpeed = 3f;      // 회전 속도 (조향 강도)
    public float maxTurnAngle = 30f;    // 최대 회전 각도
    public float brakeSpeed = 3f;      // 브레이크 시 감속 속도
    private Rigidbody2D rb;

    private float currentSpeed;
    private float steeringAngle;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 입력받기 (수평, 수직)
        float moveInput = Input.GetAxis("Vertical");  // W/S 키 또는 화살표 키 (앞뒤)
        float turnInput = Input.GetAxis("Horizontal"); // A/D 키 또는 화살표 키 (좌우 회전)

        // 앞뒤 이동에 따른 속도 계산
        currentSpeed = moveInput * moveSpeed;

        // 회전 각도 계산
        steeringAngle = turnInput * maxTurnAngle;

        // 자동차의 회전 처리 (2D에서 회전은 Z축을 기준으로)
        if (currentSpeed != 0)
        {
            // 회전량을 계산: 자동차가 속도가 있을 때만 회전
            float turnAmount = steeringAngle * (Mathf.Abs(currentSpeed) / moveSpeed);
            rb.rotation -= turnAmount * Time.deltaTime * turnSpeed;  // 회전 속도에 비례하여 회전
        }

        // 자동차 이동 (속도에 맞게 이동 방향 설정)
        rb.linearVelocity = transform.right * currentSpeed;  // X축을 따라 이동

        // 브레이크 기능 (스페이스바)
        if (Input.GetKey(KeyCode.Space))  // 스페이스바를 누르면 브레이크
        {
            currentSpeed -= brakeSpeed * Time.deltaTime;
            currentSpeed = Mathf.Max(currentSpeed, 0);  // 속도가 0보다 적어지지 않도록
        }
    }
}
