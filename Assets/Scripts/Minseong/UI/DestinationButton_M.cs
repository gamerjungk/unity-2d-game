using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DestinationButton_M : MonoBehaviour
{
    [SerializeField] TMP_Text label;

    int index;
    DestinationUI_M ui;

    public void Init(int idx, DestinationUI_M parent)
    {
        index = idx;
        ui = parent;
        GetComponent<Button>().onClick.AddListener(() => ui.SelectIndex(index));
    }

    public void SetLabel(string txt) => label.text = txt;

    /* ─ 픽업 / 배달 단계에 따른 글자 색상 ─ */
    public void SetStage(bool isPickup)
    {
        label.color = isPickup
            ? Color.black                                  // 픽업 : 흰색
            : new Color(0.12f, 0.55f, 1f);               // 배달 : 푸른 계열
    }
}
