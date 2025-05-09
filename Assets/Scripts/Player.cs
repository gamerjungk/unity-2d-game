using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D playerRigid;
    public float acceleration = 5f, maxSpeed = 20f, deceleration = 3f;     // 가속도, 최고속도, 감속도
    private float currentSpeed = 0f, currentAngle = 0f, targetAngle = 0f;
    public float steerSensitivity = 2f;
    void Start()
    {
        playerRigid = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, currentAngle + 90f); // 차량 정면을 맞추기 위한 +90 보정

        if(GameManager.inst.turnManager.isMidTurn){

        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
        transform.position += transform.right * currentSpeed * Time.deltaTime;  // currentSpeed 비례 이동

        } else currentSpeed = 0f;
    }
    public void Accelerate(float force)
    {
        currentSpeed += force * acceleration * Time.deltaTime;
    }
    public void Brake()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, 0f, deceleration * Time.deltaTime);
    }
    public void Handling(float angle) {
        targetAngle = angle;
        currentAngle = Mathf.Lerp(currentAngle, targetAngle, steerSensitivity * Time.deltaTime);
    }

}