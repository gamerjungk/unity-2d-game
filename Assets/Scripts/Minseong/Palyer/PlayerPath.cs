using UnityEngine;

public class PlayerPath : MonoBehaviour
{
    private int pathIndex = 0;

    void Start()
    {
        PathGuide2.Instance.SetDestination(transform);
    }

    void Update()
    {
        if (PathGuide2.Instance == null || PathGuide2.Instance.lineRenderer.positionCount == 0)
            return;

        Vector3 target = PathGuide2.Instance.lineRenderer.GetPosition(pathIndex);
        Vector3 flatPos = new Vector3(transform.position.x, target.y, transform.position.z); // Y�� ����

        if (Vector3.Distance(flatPos, target) < 1.0f) // ���� ���� �Ÿ�
        {
            pathIndex++;

            if (pathIndex >= PathGuide2.Instance.lineRenderer.positionCount)
            {
                Invoke(nameof(SetNewTarget), 1f);
            }
        }
    }

    void SetNewTarget()
    {
        pathIndex = 0;
        PathGuide2.Instance.SetDestination(transform);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destination"))
        {
            Destroy(other.gameObject);
            pathIndex = 0;
            PathGuide2.Instance.SetDestination(transform);
        }
    }

}
