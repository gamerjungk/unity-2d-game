using UnityEngine;

public class PlayerPath : MonoBehaviour
{
    [SerializeField] private float arrivalRadius = 1.2f;   // 필요시 조정

    private void Update()
    {
        var dm = DestinationManager.Instance;
        if (dm == null) return;

        Transform dest = dm.DestinationMarker;
        if (dest == null) return;

        if (Vector3.Distance(transform.position, dest.position) <= arrivalRadius)
        {
            dm.MoveDestination();
        }
    }
}
