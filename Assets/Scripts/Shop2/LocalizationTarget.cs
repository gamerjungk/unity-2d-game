using TMPro;
using UnityEngine;

public class LocalizationTarget : MonoBehaviour
{
    [HideInInspector] public string key;  // 드롭다운은 에디터에서 처리
    private TextMeshProUGUI tmp;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        RefreshText();
    }

    public void RefreshText()
    {
        if (tmp == null) tmp = GetComponent<TextMeshProUGUI>();
        if (tmp != null) tmp.text = LocalizationManager.Instance.GetText(key);
    }
}
