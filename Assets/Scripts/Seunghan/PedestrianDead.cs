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

/*
pedestrian의 충돌 처리 스크립트
충돌 시 Hit 애니메이션 트리거 설정
충돌 후 Rigidbody를 비활성화하고 Collider를 비활성화하여 물리적 상호작용 방지
충돌 후 isDead 상태로 설정하여 추가 충돌 방지
*/
