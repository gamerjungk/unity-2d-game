using UnityEngine;

public class PlayerPath : MonoBehaviour
{
    [SerializeField] private float arriveRadius = 1.2f;

    private void Update()
    {
        var dm = DestinationManager.Instance;
        if (dm == null || dm.CurrentTarget == null) return;

        if (Vector3.Distance(transform.position, dm.CurrentTarget.position) <= arriveRadius)
        {
            dm.ArrivedCurrentTarget();
        }
    }
}
