using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DestinationButton_M : MonoBehaviour
{
    [SerializeField] TMP_Text label;

    // 색상 추가(3줄)
    [Header("Text Colors")]
    [SerializeField] Color pickupColor = Color.black;     // 기본(픽업지) 색
    [SerializeField] Color deliveryColor = new(0.12f, 0.55f, 1f); // 배달지 색

    int index;
    DestinationUI_M ui;

    /* ─────────────────────────────────────────────── */
    /* 초기화                                          */
    /* ─────────────────────────────────────────────── */
    public void Init(int idx, DestinationUI_M parent)
    {
        index = idx;
        ui = parent;
        GetComponent<Button>()
            .onClick.AddListener(() => ui.SelectIndex(index));
    }

    public void SetLabel(string text)
    {
        label.text = text;
    }

    // 픽업/배달에 따른 색 변경 추가
    public void SetAsPickup() => label.color = pickupColor;
    public void SetAsDelivery() => label.color = deliveryColor;

}
