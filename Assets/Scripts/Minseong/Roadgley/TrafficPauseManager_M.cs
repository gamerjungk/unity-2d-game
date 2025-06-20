using System.Collections.Generic;
using UnityEngine;
using Gley.TrafficSystem;   // VehicleComponent가 들어있는 네임스페이스

public class TrafficPauseManager_M : MonoBehaviour
{
    static bool paused;
    static readonly List<(Rigidbody rb, Vector3 v, Vector3 w)> cached = new(); // 저장용

    public static void SetPaused(bool value)
    {
        if (paused == value) return;
        paused = value;

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
                var rb = car.rb;

                // 기존 버전에 따라 linearVelocity/velocity 분기
#if UNITY_6000_0_OR_NEWER
                cached.Add((rb, rb.linearVelocity, rb.angularVelocity));
                rb.linearVelocity = Vector3.zero;
#else
                cached.Add((rb, rb.velocity, rb.angularVelocity));
                rb.velocity = Vector3.zero;
#endif
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;     // 물리 계산 정지
            }
        }
        else
        {
            // 언파즈 시, 캐시에 저장된 원래 속도로 복구
            foreach (var (rb, v, w) in cached)
            {
                if (rb == null) continue;
                rb.isKinematic = false;

#if UNITY_6000_0_OR_NEWER
                rb.linearVelocity = v;
#else
                rb.velocity       = v;
#endif
                rb.angularVelocity = w;
            }
            cached.Clear();
        }
    }
}
