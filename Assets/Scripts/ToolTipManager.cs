using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    public Vector2 offset = new Vector2(10f, -10f);

    private void Awake()
    {
        Instance = this;
        HideTooltip();
    }

    private void Update()
    {
        if (tooltipPanel.activeSelf)
        {
            Vector2 mousePosition = Input.mousePosition;
            tooltipPanel.transform.position = (Vector3)(mousePosition + offset);
        }
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