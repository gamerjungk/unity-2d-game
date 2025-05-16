using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MiniMapFollow : MonoBehaviour
{
    public Transform target;

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 newPosition = target.position;
            newPosition.y = transform.position.y; // 높이 고정
            transform.position = newPosition;

            // 방향도 동일하게 맞춤 (Y축 회전만 반영)
            transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
        }
    }
}
