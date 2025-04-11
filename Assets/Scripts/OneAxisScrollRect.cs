using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OneAxisScrollRect : ScrollRect
{
    public enum ScrollMode { Auto, VerticalOnly, HorizontalOnly }
    
    [SerializeField]
    public ScrollMode mode = ScrollMode.Auto;

    public override void OnScroll(PointerEventData data)
    {
        if (!IsActive())
            return;

        Vector2 scrollDelta = data.scrollDelta;

        // macOS 터치패드는 Y 반전 필요
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        scrollDelta.y *= -1;
#endif

        switch (mode)
        {
            case ScrollMode.VerticalOnly:
                scrollDelta.x = 0;
                break;
            case ScrollMode.HorizontalOnly:
                scrollDelta.y = 0;
                break;
            case ScrollMode.Auto:
                if (Mathf.Abs(scrollDelta.y) > Mathf.Abs(scrollDelta.x))
                    scrollDelta.x = 0; // 세로 우선
                else
                    scrollDelta.y = 0; // 가로 우선
                break;
        }

        // 직접 수정된 scrollDelta로 base 호출
        Vector2 originalDelta = data.scrollDelta;
        data.scrollDelta = scrollDelta;

        base.OnScroll(data);

        // 원래대로 되돌려서 부작용 방지
        data.scrollDelta = originalDelta;
    }
}
