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
            cached.Clear();
            foreach (var car in FindObjectsOfType<VehicleComponent>())
            {
                var rb = car.rb;
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
            foreach (var (rb, v, w) in cached)
            {
                if (rb == null) continue;      // 이미 풀에 반납된 경우
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
