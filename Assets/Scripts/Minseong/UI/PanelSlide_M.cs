using UnityEngine;
using System.Collections;

public class PanelSlide_M : MonoBehaviour
{
    [Header("설정")] // 헤더 표시
    [Range(0, 1)] public float hiddenRatio = 0.6f; // 숨겨질 비율(패널 높이의 몇 %를 가릴지)
    public float animTime = .25f; // 슬라이드 애니메이션 시간(초)
    public float edgeClickHeight = 20f; // 숨김 상태에서 터치 영역으로 남길 높이(px)

    RectTransform rt; // 패널의 RectTransform 캐시
    Vector2 shownPos; // 보인 상태의 앵커 위치
    Vector2 hiddenPos; // 숨겨진 상태의 앵커 위치
    bool isShown = true; // 현재 패널이 보인 상태인지 플래그 (초기값: 보임)
    Coroutine co; // 현재 실행 중인 애니메이션 코루틴 참조

    void Awake()
    {
        rt = GetComponent<RectTransform>(); // RectTransform 컴포넌트 가져오기

        /* ───── 위치 계산 ───── */
        shownPos = rt.anchoredPosition; // 현재 앵커 위치를 보인 상태 위치로 저장

        float hidePixels = rt.rect.height * hiddenRatio; // 숨길 픽셀 수 계산
        // 보인 위치에서 아래로 이동해 숨겨질 위치 계산
        hiddenPos = shownPos + Vector2.down * (hidePixels + edgeClickHeight);

        /* 처음엔 숨김 상태로 설정 */
        rt.anchoredPosition = hiddenPos; // 앵커 위치를 숨긴 위치로 이동
        isShown = false; // 현재는 숨김 상태로 표시
    }

    // 외부에서 클릭 이벤트를 받는 메서드 (EventTrigger → PointerClick)
    public void OnPointerClick()
    {
        // 샐행 중인 코루틴이 있으면 중단하여 중복 실행 방지
        if (co != null) StopCoroutine(co);
        // Animate 코루틴 시작 후 참조 저장
        co = StartCoroutine(Animate());
    }

    // 패널 슬라이드 애니메이션을 처리하는 코루틴
    IEnumerator Animate()
    {
        isShown = !isShown; // 보임/숨김 상태 토글
        Vector2 from = rt.anchoredPosition; // 현재 시작 위치 저장
        Vector2 to = isShown ? shownPos : hiddenPos; // 목표 위치 결정

        float t = 0; // 보간 파라미터 초기화 (0→1)

        // 필요한 경우: CanvasGroup이 있으면 클릭 차단 설정
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg) cg.blocksRaycasts = isShown;  // 보일 때만 Raycast 허용

        while (t < 1f) // t가 1보다 작으면 루프 계속
        {
            t += Time.unscaledDeltaTime / animTime; // 언스케일드 시간으로 t 증가
            float k = Mathf.SmoothStep(0f, 1f, t); // SmoothStep으로 부드럽게 보간

            // 앵커 위치를 시작 위치에서 목표 위치로 보간
            rt.anchoredPosition = Vector2.Lerp(from, to, k);

            yield return null; // 다음 프레임까지 대기
        }

        co = null; // 애니메이션 완료 후 코루틴 참조 해제
    }
}
