using UnityEngine;
using UnityEngine.AI;

public class PedestrianAnimatorController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float speed = agent.velocity.magnitude;

        // Sprint 조건 (속도 기준)
        if (speed > 3.5f)
        {
            animator.SetBool("isSprinting", true);
            animator.SetBool("isWalking", false);
        }
        // Walk 조건
        else if (speed > 0.1f)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isSprinting", false);
        }
        // Idle 조건
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isSprinting", false);
        }
    }
}
