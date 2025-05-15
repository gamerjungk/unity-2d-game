using UnityEngine;

public class PlayerMove_M : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 100f;

    private Rigidbody rb;
    private Vector2 touchStartPos;
    private bool isTouching = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float move = 0f;
        float turn = 0f;

#if UNITY_EDITOR || UNITY_STANDALONE
        // PC/에디터 키보드 조작
        move = Input.GetAxis("Vertical") * moveSpeed;
        turn = Input.GetAxis("Horizontal") * turnSpeed;

#elif UNITY_ANDROID || UNITY_IOS
        // 모바일 터치 조작
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    isTouching = true;
                    break;
                case TouchPhase.Moved:
                    if (isTouching)
                    {
                        Vector2 delta = touch.position - touchStartPos;
                        move = delta.y / Screen.height * moveSpeed;
                        turn = delta.x / Screen.width * turnSpeed;
                    }
                    break;
                case TouchPhase.Ended:
                    isTouching = false;
                    break;
            }
        }
#endif

        // 실제 이동
        rb.MovePosition(rb.position + transform.forward * move * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn * Time.fixedDeltaTime, 0f));
    }
}
