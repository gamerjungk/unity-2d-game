using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 200f;
    private Rigidbody2D rb;
    private float moveInput;
    private float turnInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    void Update()
    {
        moveInput = Input.GetAxis("Vertical");   // ↑↓ 키 입력
        turnInput = Input.GetAxis("Horizontal"); // ←→ 키 입력
    }

    void FixedUpdate()
    {
        rb.linearVelocity = transform.right * moveInput * speed;
        rb.angularVelocity = -turnInput * rotationSpeed;
    }
    public void Die()
    {
        gameObject.SetActive(false);
        Debug.Log("EndGame start");
        if (GameManager.inst != null)
        {
            GameManager.inst.EndGame();
        }
        else
        {
            Debug.LogWarning("GameManager를 찾을 수 없습니다.");
        }
    }
}
