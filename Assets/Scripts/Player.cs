using UnityEngine;

public class Player : MonoBehaviour
{

    public float acceleration = 5f, maxSpeed = 20f, deceleration = 3f;
    public float currentSpeed = 0f, currentAngle = 0f, targetAngle = 0f;
    public float steerSensitivity = 3f;

    void Start()
    {
        currentAngle = transform.eulerAngles.y;
    }

    void FixedUpdate()
    {
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

    public void Brake()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, 0f, deceleration * Time.deltaTime);
    }

    public void Handling(float angle)
    {
        targetAngle = -angle;
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("GasStation") && GameManager.inst.uiManager.gearState == 1 && GameManager.fuel < 69)
        {
            currentSpeed = 0;
            GameManager.inst.turnManager.midTurn();
            GameManager.fuel += Time.deltaTime;
        }
    }
}