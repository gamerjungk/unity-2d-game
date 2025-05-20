using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    public Vector2 offset = new Vector2(20f, -20f);

    private RectTransform tooltipRect;
    private Canvas canvas;

    private void Awake()
    {
        Instance = this;
        tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        HideTooltip();
    }

    private void Update()
    {
        if (tooltipPanel.activeSelf)
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 anchoredPos;

            // 스크린 → 로컬 좌표 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                mousePos,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out anchoredPos
            );

            // 오프셋 적용
            anchoredPos += offset;

            // 툴팁이 캔버스를 넘지 않도록 클램프
            Vector2 clampedPos = ClampToCanvas(anchoredPos);

            tooltipRect.anchoredPosition = clampedPos;
        }
    }

    private Vector2 ClampToCanvas(Vector2 position)
    {
        if (canvas == null || tooltipRect == null)
            return position;

        RectTransform canvasRect = canvas.transform as RectTransform;
        Vector2 size = tooltipRect.sizeDelta;

        // 좌측 하단 기준 Clamp
        float minX = -canvasRect.rect.width * 0.5f;
        float maxX = canvasRect.rect.width * 0.5f - size.x;

        float minY = -canvasRect.rect.height * 0.5f + size.y;
        float maxY = canvasRect.rect.height * 0.5f;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        return position;
    }

    public void ShowTooltip(string message)
    {
        tooltipText.text = message;
        tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}
