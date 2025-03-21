using UnityEngine;

public class CarObj : MonoBehaviour
{
    // public float speed = 5f;
    // public float rotationSpeed = 200f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("MoveRandomly",1f,1f);
    }
    void MoveRandomly()
    {
        float randomDistance = Random.Range(1f,5f);
        rb.MovePosition(rb.position +(Vector2)transform.right * randomDistance);
    }
    void crash()
    {
        Debug.Log("Crash!");
    }

    void OnCollisionEnter2D(Collision2D other)
    {  // ✅ 수정 (Collider2D → Collision2D)
        crash();
        if(other.gameObject.CompareTag("Player")){
            CarController carController = other.gameObject.GetComponent<CarController>();

            if(carController != null){
                Debug.Log("플레이어와 충돌 감지됨! Die() 실행");
                carController.Die();
            }
        }
    }
}


    