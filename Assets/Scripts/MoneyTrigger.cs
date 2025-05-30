using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MoneyTrigger : MonoBehaviour
{
    bool rewarded;

    void OnTriggerEnter(Collider other)
    {
        if (rewarded || !other.CompareTag("Player")) return;

        rewarded = true;                                   // 중복 방지
        DestinationManager.Instance.ArrivedCurrentTarget();
    }

    public void ResetTrigger() => rewarded = false;        // DestinationManager 에서 호출
}
