using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

// 툴팁 UI를 제어하는 싱글톤 매니저 클래스
public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;  // 전역 접근을 위한 인스턴스

    public GameObject tooltipPanel;         // 툴팁 UI 패널 오브젝트    
    public TextMeshProUGUI tooltipText;     // 툴팁에 표시될 텍스트
    public Vector2 offset = new Vector2(20f, -20f); // 마우스 기준 툴팁 위치 오프셋

    private RectTransform tooltipRect;  // 툴팁 패널의 RectTransform
    private Canvas canvas;  // 부모 캔버스 참조

    private void Awake()
    {
        Instance = this;    // 싱글톤 인스턴스 설정
        tooltipRect = tooltipPanel.GetComponent<RectTransform>();   // 툴팁 RectTransform 가져오기
        canvas = GetComponentInParent<Canvas>();    // 상위 캔버스 찾기
        HideTooltip();  // 초기 상태에서 툴팁 숨김
    }

    private void Update()
    {   
        // 툴팁이 활성 상태일 경우 위치를 계속 마우스에 맞춰 갱신
        if (tooltipPanel.activeSelf)
        {
            Vector2 mousePos = Input.mousePosition; // 현재 마우스 위치
            Vector2 anchoredPos;

            // 스크린 좌표 → 캔버스 로컬 좌표로 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                mousePos,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out anchoredPos
            );

            // 오프셋 적용
            anchoredPos += offset;  // 오프셋 적용

            // 툴팁이 화면 밖으로 나가지 않도록 제한
            Vector2 clampedPos = ClampToCanvas(anchoredPos);

            tooltipRect.anchoredPosition = clampedPos;  // 위치 적용
        }
    }

    // 툴팁 위치가 캔버스를 벗어나지 않도록 제한
    private Vector2 ClampToCanvas(Vector2 position)
    {
        if (canvas == null || tooltipRect == null)
            return position;

        RectTransform canvasRect = canvas.transform as RectTransform;
        Vector2 size = tooltipRect.sizeDelta;   // 툴팁 크기

        // 좌측 하단 기준 Clamp
        float minX = -canvasRect.rect.width * 0.5f;
        float maxX = canvasRect.rect.width * 0.5f - size.x;

        float minY = -canvasRect.rect.height * 0.5f + size.y;
        float maxY = canvasRect.rect.height * 0.5f;

        // 위치 보정
        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        return position;
    }

    // 툴팁 텍스트 표시 및 활성화
    public void ShowTooltip(string message)
    {
        tooltipText.text = message;
        tooltipPanel.SetActive(true);
    }

    // 툴팁 숨기기
    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}
