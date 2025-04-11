using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D playerRigid;
    public float acceleration = 5f;     // 가속력
    public float maxSpeed = 20f;        // 최고 속도
    public float deceleration = 3f;     // 감속력 (마찰)
    public float turnSpeed = 50f;       // 회전 속도
    private Vector3 currentSpeed = Vector3.zero;
    float moveInput = 0, turnInput = 0;
    void Start()
    {
        playerRigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(GameManager.inst.turnManager.isMidTurn){
        if (moveInput != 0)        // 가속/감속
        {
            currentSpeed += transform.right * moveInput * acceleration * Time.deltaTime;
        }
        else            // 마찰력 적용 (자연 감속)
        {
            currentSpeed = Vector3.Lerp(currentSpeed, Vector3.zero, deceleration * Time.deltaTime);
        }
        currentSpeed = Vector3.ClampMagnitude(currentSpeed, maxSpeed);

        if (currentSpeed.magnitude > 0.1f)
        {
            float turn = turnInput * turnSpeed * Time.deltaTime;
            transform.Rotate(0, turn, 0);
        }
        transform.position += currentSpeed * Time.deltaTime;  

        } else currentSpeed = Vector3.zero;
    }
    //0403���� Accelerater,brake
    public void Accelerate(float force)
    {
        moveInput = force;
    }
    public void Brake(float intensity)
    {
        moveInput = Mathf.Max(moveInput - intensity, 0);
    }

}
