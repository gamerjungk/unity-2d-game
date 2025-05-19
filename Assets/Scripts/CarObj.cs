using UnityEngine;

public class CarObj : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void MoveRandomly()
    {
        float randomDistance = Random.Range(1f,2f);
        rb.MovePosition(rb.position +(Vector2)transform.right * randomDistance);
    }
}


    