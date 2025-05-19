using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D playerRigid;
    public float acceleration = 5f, maxSpeed = 20f, deceleration = 3f;     // 가속도, 최고속도, 감속도
    public float currentSpeed = 0f, currentAngle = 0f, targetAngle = 0f;
    public float steerSensitivity = 2f;
    void Start()
    {
        playerRigid = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if(GameManager.inst.turnManager.isMidTurn){
            if (GameManager.fuel >= currentSpeed * 0.1f * Time.deltaTime)
            {
                GameManager.fuel -= currentSpeed * 0.1f * Time.deltaTime;
                currentSpeed = Mathf.Lerp(currentSpeed, currentSpeed * 0.5f, deceleration * 0.1f * Time.deltaTime);
                currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);
            }
            else
            {
                currentSpeed = Mathf.Lerp(currentSpeed, 0f, deceleration * 0.1f * Time.deltaTime);
            }
            transform.rotation = Quaternion.Euler(0f, 0f, currentAngle + 90f); // 차량 정면을 맞추기 위한 +90 보정
            currentAngle = Mathf.Lerp(currentAngle, targetAngle, Mathf.Min(Mathf.Abs(currentSpeed * 0.5f), 2f) * Time.deltaTime);
            transform.position += transform.right * currentSpeed * Time.deltaTime;  // currentSpeed 비례 이동
        }
    }
    public void Accelerate(float force)
    {
        if(force >= 0) currentSpeed += force * acceleration * Time.deltaTime;
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
    public void Handling(float angle) {
        targetAngle = angle;
    }

}