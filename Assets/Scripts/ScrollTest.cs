using UnityEngine;
using UnityEngine.UI;

public class ScrollTest : MonoBehaviour
{
    public ScrollRect scrollRect;

    void Start()
{
    scrollRect.verticalNormalizedPosition = 0.5f; // 실행 시작 시 중간 위치로
}
    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            Debug.Log("휠 입력 감지됨: " + scroll);

            // 스크롤 이동 (위로 올리면 위로 이동하도록 방향 반전)
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(
                scrollRect.verticalNormalizedPosition - scroll * 0.2f
            );
        }
    }
}
