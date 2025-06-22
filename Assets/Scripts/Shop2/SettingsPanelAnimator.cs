using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsPanelAnimator : MonoBehaviour
{
    public GameObject panelRoot;           // 전체 설정 패널의 루트 오브젝트
    public CanvasGroup panelGroup;         // 패널의 페이드 효과 및 인터랙션 제어용
    public RectTransform panelRect;        // 슬라이드 이동에 사용할 패널 RectTransform
    public float slideDuration = 2.0f;     // 패널 슬라이드 애니메이션 지속 시간

    private Vector2 startHiddenPos;        // 왼쪽 화면 밖 숨김 시작 위치
    private Vector2 endHiddenPos;          // 오른쪽 화면 밖 숨김 종료 위치
    private Vector2 visiblePos;            // 화면 중앙에 표시될 위치

    private void Awake()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect); // 레이아웃 갱신

        float panelWidth = panelRect.rect.width;
        visiblePos = Vector2.zero;                              // 중앙 위치는 (0, 0)
        startHiddenPos = new Vector2(-panelWidth, 0);           // 왼쪽 숨김 시작 위치 계산
        endHiddenPos = new Vector2(panelWidth, 0);              // 오른쪽 숨김 종료 위치 계산

        panelRect.anchoredPosition = startHiddenPos;            // 초기 위치를 숨김 상태로 설정
        panelGroup.alpha = 0f;                                  // 초기 투명도 0
        panelGroup.interactable = false;                        // 초기 상호작용 비활성화
        panelGroup.blocksRaycasts = false;                      // 초기 클릭 차단 안 함
        panelRoot.SetActive(false);                             // 패널 비활성화
    }

    public void OpenPanel()
    {
        StopAllCoroutines();                                    // 기존 애니메이션 중지
        StartCoroutine(OpenWithDelay());                        // 열기 코루틴 시작
    }

    private IEnumerator OpenWithDelay()
    {
        panelRoot.SetActive(true);                              // 패널 활성화
        yield return null;                                      // 한 프레임 대기 (UI 초기화 대기)
        yield return AnimatePanel(startHiddenPos, visiblePos, fadeIn: true);
    }

    public void ClosePanel()
    {
        StopAllCoroutines();                                    // 기존 애니메이션 중지
        StartCoroutine(AnimatePanel(visiblePos, endHiddenPos, fadeIn: false, onComplete: () =>
        {
            panelRoot.SetActive(false);                         // 종료 후 패널 비활성화
        }));
    }

    private IEnumerator AnimatePanel(Vector2 from, Vector2 to, bool fadeIn, System.Action onComplete = null)
    {
        float elapsed = 0f;
        float startAlpha = fadeIn ? 0f : 1f;                    // 시작 투명도 설정
        float endAlpha = fadeIn ? 1f : 0f;                      // 종료 투명도 설정

        panelRect.anchoredPosition = from;                      // 시작 위치 설정
        panelGroup.alpha = startAlpha;                          // 시작 알파값 설정
        panelGroup.interactable = fadeIn;                       // 상호작용 가능 여부 설정
        panelGroup.blocksRaycasts = fadeIn;                     // 클릭 차단 여부 설정

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);   // 0~1 구간 보간 인자 계산

            panelRect.anchoredPosition = Vector2.Lerp(from, to, t); // 위치 슬라이드
            panelGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t); // 알파값 점점 바꿈
            yield return null;
        }

        panelRect.anchoredPosition = to;                        // 최종 위치 설정
        panelGroup.alpha = endAlpha;                            // 최종 투명도 설정
        panelGroup.interactable = fadeIn;                       // 상호작용 상태 최종 반영
        panelGroup.blocksRaycasts = fadeIn;

        onComplete?.Invoke();                                   // 콜백 실행 (패널 비활성화 등)
    }
}
