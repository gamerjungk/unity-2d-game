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
}
