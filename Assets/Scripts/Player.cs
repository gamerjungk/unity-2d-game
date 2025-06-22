using UnityEngine;

public class Player : MonoBehaviour
{

    public float acceleration = 5f, maxSpeed = 20f, deceleration = 3f;
    public float currentSpeed = 0f, currentAngle = 0f, targetAngle = 0f;
    public float steerSensitivity = 3f;

    // 시작할 때 플레이어의 각도 초기화
    void Start()
    {
        currentAngle = transform.eulerAngles.y;
    }

    void FixedUpdate()
    {
        // 턴이 진행중일 때 조작 처리
        if (GameManager.inst.turnManager.isMidTurn)
        {
            // 연료 소모 및 감속 처리
            if (GameManager.fuel >= currentSpeed * 0.01f * Time.deltaTime)
            {
                GameManager.fuel -= Mathf.Abs(currentSpeed) * 0.01f * Time.deltaTime;
                currentSpeed = Mathf.Lerp(currentSpeed, currentSpeed * 0.5f, deceleration * 0.1f * Time.deltaTime);
                currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);
            }
            else
            {
                currentSpeed = Mathf.Lerp(currentSpeed, 0f, deceleration * 0.1f * Time.deltaTime);
            }

            // 회전 처리 (Y축)
            currentAngle = Mathf.Lerp(currentAngle, targetAngle, Mathf.Min(Mathf.Abs(currentSpeed * 0.5f), 2f) * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, currentAngle, 0f);

            // 전진
            transform.position += transform.forward * currentSpeed * Time.deltaTime;
        }
    }

    // 플레이어 가속
    public void Accelerate(float force)
    {
        if (force >= 0)
        {
            currentSpeed += force * acceleration * Time.deltaTime;
        }
        else
        {
            if (currentSpeed <= 2f)
            {
                currentSpeed += force * acceleration * Time.deltaTime;
            }
            else Debug.Log("운행 중 R기어! 고장!");
        }
    }

    // 플레이어 감속
    public void Brake()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, 0f, deceleration * Time.deltaTime);
    }

    public void Handling(float deltaAngle) // 변화량 전달
    {
        targetAngle -= deltaAngle * steerSensitivity; //후 저장해서 각도를 설정함
    }
    // 고정 각도 사용 (기존 내용)
    // public void Handling(float angle)
    // {
    //     targetAngle = -angle;
    // }

    // 플레이어가 특정 트리거에 존재할 경우 처리
    void OnTriggerStay(Collider other)
    {
        // 주유소 트리거에서 P기어로 조작 시 주유 시작. 턴 소비
        if (other.CompareTag("GasStation") && GameManager.inst.uiManager.gearState == 1 && GameManager.fuel < 69)
        {
            currentSpeed = 0;
            GameManager.inst.turnManager.midTurn();
            GameManager.fuel += Time.deltaTime;
            Debug.Log("주유소 발동!");
        }
    }
}