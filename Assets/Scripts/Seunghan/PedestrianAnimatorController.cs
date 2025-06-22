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

/*
pedestrian의 애니메이션 제어 스크립트
NavMesh의 속도를 기반으로 애니메이션 상태 변경
속도에 따라 boolean으로 걷기, 달리기, 대기 상태를 전환
*/