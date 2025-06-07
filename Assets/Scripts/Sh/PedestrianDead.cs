using UnityEngine;

public class PedestrianController : MonoBehaviour
{
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Hit();
        }
    }

    void Hit()
    {
        isDead = true;
        animator.SetTrigger("Hit");  // GleyPedestrianController에서 Hit로 die가 발생되도록 설정되어 있음

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
}
