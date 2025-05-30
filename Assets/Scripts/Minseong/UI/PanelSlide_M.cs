using UnityEngine;
using System.Collections;

public class PanelSlide_M : MonoBehaviour
{
    [Header("설정")]
    [Range(0, 1)] public float hiddenRatio = 0.6f; // 몇 % 가릴지
    public float animTime = .25f;
    public float edgeClickHeight = 20f;            // 숨김 상태에서 터치될 영역(px)

    RectTransform rt;
    Vector2 shownPos;
    Vector2 hiddenPos;
    bool isShown = true;       // 시작은 ‘올라온’ 상태
    Coroutine co;

    void Awake()
    {
        rt = GetComponent<RectTransform>();

        /* ───── 위치 계산 ───── */
        shownPos = rt.anchoredPosition;
        float hidePixels = rt.rect.height * hiddenRatio;
        hiddenPos = shownPos + Vector2.down * (hidePixels + edgeClickHeight);
        /* 처음엔 숨김 상태로 */
        rt.anchoredPosition = hiddenPos;
        isShown = false;
    }

    public void OnPointerClick()   // EventTrigger → PointerClick
    {
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        isShown = !isShown;
        Vector2 from = rt.anchoredPosition;
        Vector2 to = isShown ? shownPos : hiddenPos;
        float t = 0;

        // 필요한 경우: 버튼 Raycast 차단
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg) cg.blocksRaycasts = isShown;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / animTime;
            rt.anchoredPosition = Vector2.Lerp(from, to, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
        co = null;
    }
}
