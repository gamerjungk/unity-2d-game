using UnityEngine;
//오류해결코드
// 해상도 고정 카메라 스크립트 (수정됨)
public class CameraResolution : MonoBehaviour
{
    public Transform target;

    void Start()
    {
        Camera camera = GetComponent<Camera>();
        Rect rect = camera.rect;

        // y를 0, h를 1로 고정
        rect.y = 0f;
        rect.height = 1f;

        float scaleheight = ((float)Screen.width / Screen.height) / ((float)9 / 16);
        float scalewidth = 1f / scaleheight;

        if (scaleheight < 1)
        {
            rect.width = scalewidth;
            rect.x = (1f - scalewidth) / 2f;
        }
        else
        {
            rect.width = 1f; // 가로 전체 사용
            rect.x = 0f;     // 왼쪽 끝부터 시작
        }
        camera.rect = rect;
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 targetPos = target.position + new Vector3(0, 2, -10);
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.2f);
        }
    }

    void OnPreCull() => GL.Clear(true, true, Color.black);
}