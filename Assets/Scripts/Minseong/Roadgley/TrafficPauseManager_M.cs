using System.Collections.Generic;
using UnityEngine;
using Gley.TrafficSystem;   // VehicleComponent가 들어있는 네임스페이스

public class TrafficPauseManager_M : MonoBehaviour
{
    // 일시정지 상태 플래그
    static bool paused;
    // Rigidbody와 선형·각속도 정보를 저장할 튜플 리스트(캐시)
    static readonly List<(Rigidbody rb, Vector3 v, Vector3 w)> cached = new(); // 저장용

    // 외부에서 호출하여 트래픽 일시정지/해제 토글
    public static void SetPaused(bool value)
    {
        if (paused == value) return; // 중복 호출 방지
        paused = value; // 상태 갱신

        if (paused)
        {
            // 1) 캐시 초기화
            cached.Clear();

            // 2) VehicleComponent 수집
#if UNITY_2023_2_OR_NEWER
            var vehicles = Object.FindObjectsByType<VehicleComponent>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );
#else
            var vehicles = FindObjectsOfType<VehicleComponent>();
#endif

            // 3) 각 차량 정지 및 속도 저장
            foreach (var car in vehicles)
            {
                var rb = car.rb; // Rigidbody 참조

                // 기존 버전에 따라 linearVelocity/velocity 분기
#if UNITY_6000_0_OR_NEWER
                cached.Add((rb, rb.linearVelocity, rb.angularVelocity)); // 속도 저장
                rb.linearVelocity = Vector3.zero; // 선형 속도 0으로
#else
                cached.Add((rb, rb.velocity, rb.angularVelocity)); // 속도 저장
                rb.velocity = Vector3.zero; // 선형 속도 0으로
#endif
                rb.angularVelocity = Vector3.zero; // 각속도 0으로
                rb.isKinematic = true;     // 물리 계산 정지
            }
        }
        else
        {
            // 언파즈 시, 캐시에 저장된 원래 속도로 복구
            foreach (var (rb, v, w) in cached)
            {
                if (rb == null) continue; // 파괴된 객체 무시
                rb.isKinematic = false; // 물리 계산 재개

#if UNITY_6000_0_OR_NEWER
                rb.linearVelocity = v; // 저장해둔 선형 속도 복원
#else
                rb.velocity       = v; // 저장해둔 선형 속도 복원
#endif
                rb.angularVelocity = w; // 저장해둔 각속도 복원
            }
            cached.Clear(); // 캐시 비우기
        }
    }
}

/*
    - Gley Traffic System API를 사용하여 차량의 움직임을 일시 정지하고 재개하는 매니저
    - SetPaused 메서드는 차량의 Rigidbody를 제어하여 움직임을 멈추거나 재개
    - paused 상태에 따라 차량의 속도와 회전 속도를 저장하고 복원
*/