using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 movement = Vector2.zero;

        // 키보드 입력 (에디터용)
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        if (moveX != 0 || moveY != 0)
        {
            movement = new Vector2(moveX, moveY).normalized;
        }
        // 터치 입력 (Simulator 및 모바일용)
        else if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            Vector2 direction = (touchPos - (Vector2)transform.position).normalized;
            movement = direction;
        }

        rb.linearVelocity = movement * speed;
    }
}