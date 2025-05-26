using UnityEngine;

public class MoneyTrigger : MonoBehaviour
{
    private bool rewarded = false;

    void OnTriggerEnter(Collider other)
    {
        if (rewarded) return;
        if (!other.CompareTag("Player")) return;

        rewarded = true;

        Vector3 currentDestination = transform.position;
        DestinationManager.Instance.OnDestinationReached(currentDestination);
    }

    public void ResetTrigger()
    {
        rewarded = false;
    }
}