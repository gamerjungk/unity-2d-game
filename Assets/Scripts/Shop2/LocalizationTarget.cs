using TMPro;
using UnityEngine;

// 특정 UI 텍스트가 LocalizationManager에서 제공하는 다국어 텍스트를 표시하도록 연결해주는 역할
public class LocalizationTarget : MonoBehaviour
{
    [HideInInspector] public string key;  // 에디터에서 드롭다운 등으로 설정할 문자열 키 (Inspector에서는 숨김)
    private TextMeshProUGUI tmp;    // 연결된 TextMeshProUGUI 컴포넌트 참조

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();  // 자신에게 붙은 TMP 컴포넌트를 찾아서 저장
    }
    private void Start()
    {
        RefreshText();  // 시작할 때 텍스트를 현재 언어에 맞춰 갱신
    }

    // 현재 설정된 언어에 맞는 텍스트로 변경
    public void RefreshText()
    {
        if (tmp == null) tmp = GetComponent<TextMeshProUGUI>(); // 누락된 경우 다시 참조 시도
        if (tmp != null) tmp.text = LocalizationManager.Instance.GetText(key);  // key에 해당하는 번역된 문자열로 텍스트 설정
    }
}
