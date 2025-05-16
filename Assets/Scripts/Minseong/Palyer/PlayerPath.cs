using UnityEngine;

public class PlayerPath : MonoBehaviour
{
    private int pathIndex = 0;

    void Start()
    {
        PathManager.Instance.SetDestination(transform);
    }

    void Update()
    {
        if (PathManager.Instance == null || PathManager.Instance.lineRenderer.positionCount == 0)
            return;

        Vector3 target = PathManager.Instance.lineRenderer.GetPosition(pathIndex);
        Vector3 flatPos = new Vector3(transform.position.x, target.y, transform.position.z); // Y축 보정

        if (Vector3.Distance(flatPos, target) < 1.0f) // 도착 간주 거리
        {
            pathIndex++;

            if (pathIndex >= PathManager.Instance.lineRenderer.positionCount)
            {
                Invoke(nameof(SetNewTarget), 1f);
            }
        }
    }

    void SetNewTarget()
    {
        pathIndex = 0;
        PathManager.Instance.SetDestination(transform);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destination"))
        {
            Destroy(other.gameObject);
            pathIndex = 0;
            PathManager.Instance.SetDestination(transform);
        }
    }

}
