using System.Collections.Generic;
using UnityEngine;
using Gley.TrafficSystem;

public class TrafficPauseManager_M : MonoBehaviour
{
    static bool paused;
    static readonly List<(Rigidbody rb, Vector3 v, Vector3 w)> cached = new();

    public static void SetPaused(bool value)
    {
        if (paused == value) return;
        paused = value;

        if (paused)
        {
            cached.Clear();

#if UNITY_2023_2_OR_NEWER
            var vehicles = Object.FindObjectsByType<VehicleComponent>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
            var vehicles = Object.FindObjectsOfType<VehicleComponent>();
#endif
            foreach (var car in vehicles)
            {
                var rb = car.rb;
                if (rb == null) continue;

#if UNITY_6000_0_OR_NEWER
                cached.Add((rb, rb.linearVelocity, rb.angularVelocity));
                rb.linearVelocity = Vector3.zero;
#else
                cached.Add((rb, rb.velocity, rb.angularVelocity));
                rb.velocity = Vector3.zero;
#endif
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
        else
        {
            // 캐시에 있던 것들 우선 복구
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
                rb.WakeUp(); // ✅ 물리 깨우기
            }
            cached.Clear();

            // ✅ 캐시에 없던 차량(새로 스폰/누락)까지 전체 언파즈 보정
#if UNITY_2023_2_OR_NEWER
            var all = Object.FindObjectsByType<VehicleComponent>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
            var all = Object.FindObjectsOfType<VehicleComponent>();
#endif
            foreach (var car in all)
            {
                var rb = car.rb;
                if (rb == null) continue;
                rb.isKinematic = false;
                rb.WakeUp();
            }

            // ✅ (선택) Gley AI가 가속을 재개하도록 미세한 ‘기동’ 트리거
            //     - 필요시 아주 작은 펄스를 준다(0이면 그대로 0 유지될 수 있어서)
            foreach (var car in all)
            {
                var rb = car.rb;
                if (rb == null) continue;
#if UNITY_6000_0_OR_NEWER
                if (rb.linearVelocity.sqrMagnitude < 0.0001f) rb.linearVelocity += car.transform.forward * 0.01f;
#else
                if (rb.velocity.sqrMagnitude < 0.0001f) rb.velocity += car.transform.forward * 0.01f;
#endif
            }
        }
    }

    // ✅ 씬 로드 후 Traffic System 스폰이 끝났을 법한 타이밍에 한 번 더 보정
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AfterSceneLoadKick()
    {
        // “언파즈 상태로 시작”을 확실히 보장
        paused = false;
        cached.Clear();

        // 한 프레임 뒤에 전체 언파즈 보정 코루틴 실행
        var go = new GameObject("[TrafficPauseKick]");
        go.hideFlags = HideFlags.HideAndDontSave;
        go.AddComponent<PauseKickHelper>();
    }

    // 내부 보조 컴포넌트
    class PauseKickHelper : MonoBehaviour
    {
        System.Collections.IEnumerator Start()
        {
            // TrafficComponent가 생길 때까지 잠깐 대기
            yield return null;
            yield return null;

            TrafficPauseManager_M.SetPaused(false); // 최종 언파즈
            Destroy(gameObject);
        }
    }
}
