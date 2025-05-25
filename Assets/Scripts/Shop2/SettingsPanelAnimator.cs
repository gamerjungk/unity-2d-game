using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsPanelAnimator : MonoBehaviour
{
    public GameObject panelRoot;           // 전체 패널 루트
    public CanvasGroup panelGroup;         // 페이드 인/아웃 및 블러용
    public RectTransform panelRect;        // 이동 대상 패널
    public float slideDuration = 2.0f;     // 애니메이션 지속 시간 (초)

    private Vector2 startHiddenPos;        // 시작 감춤 위치 (왼쪽 밖)
    private Vector2 endHiddenPos;          // 종료 감춤 위치 (오른쪽 밖)
    private Vector2 visiblePos;            // 화면 중앙 위치

    private void Awake()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect); // 먼저 레이아웃 갱신

        float panelWidth = panelRect.rect.width;
        visiblePos = Vector2.zero;
        startHiddenPos = new Vector2(-panelWidth, 0);
        endHiddenPos = new Vector2(panelWidth, 0);

        panelRect.anchoredPosition = startHiddenPos;
        panelGroup.alpha = 0f;
        panelGroup.interactable = false;
        panelGroup.blocksRaycasts = false;
        panelRoot.SetActive(false);
    }

    public void OpenPanel()
    {
        StopAllCoroutines();
        StartCoroutine(OpenWithDelay());
    }

    private IEnumerator OpenWithDelay()
    {
        panelRoot.SetActive(true);
        yield return null; // 한 프레임 대기 (UI 초기화 대기)
        yield return AnimatePanel(startHiddenPos, visiblePos, fadeIn: true);
    }

    public void ClosePanel()
    {
        StopAllCoroutines();
        StartCoroutine(AnimatePanel(visiblePos, endHiddenPos, fadeIn: false, onComplete: () =>
        {
            panelRoot.SetActive(false);
        }));
    }

    private IEnumerator AnimatePanel(Vector2 from, Vector2 to, bool fadeIn, System.Action onComplete = null)
    {
        float elapsed = 0f;
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        panelRect.anchoredPosition = from;
        panelGroup.alpha = startAlpha;
        panelGroup.interactable = fadeIn;
        panelGroup.blocksRaycasts = fadeIn;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);

            panelRect.anchoredPosition = Vector2.Lerp(from, to, t);
            panelGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        panelRect.anchoredPosition = to;
        panelGroup.alpha = endAlpha;
        panelGroup.interactable = fadeIn;
        panelGroup.blocksRaycasts = fadeIn;

        onComplete?.Invoke();
    }
}
