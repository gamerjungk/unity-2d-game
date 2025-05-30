using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DestinationUI_M : MonoBehaviour        
{
    [Header("External refs")]
    [SerializeField] DestinationManager manager;    // DestinationSystem 밑의 매니저
    [SerializeField] RectTransform panel;      // DestPanel
    [SerializeField] DestinationButton_M buttonPrefab;

    readonly List<DestinationButton_M> buttons = new();
    CanvasGroup cg;
    int selectedIdx = -1;

    void Awake()
    {
        cg = panel.GetComponent<CanvasGroup>() ?? panel.gameObject.AddComponent<CanvasGroup>();
        cg.interactable = true;                   // 터치 허용
        cg.blocksRaycasts = true;
    }

    void Start()
    {
        for (int i = 0; i < manager.Markers.Length; i++)
        {
            var btn = Instantiate(buttonPrefab, panel);
            btn.Init(i, this);                      // ↓ 버튼쪽 Init 시그니처와 맞춰주세요
            buttons.Add(btn);
        }
    }

    void Update()
    {
        Vector3 playerPos = manager.Player.position;

        for (int i = 0; i < buttons.Count; i++)
        {
            float dist = Vector3.Distance(playerPos, manager.Markers[i].position);
            buttons[i].SetLabel($"배달지{i + 1}   {dist:0} m");

            Color c = buttons[i].GetComponent<Image>().color;
            c.a = (i == selectedIdx) ? 1f : 0.55f;   // 선택-버튼만 진하게
            buttons[i].GetComponent<Image>().color = c;
        }
    }

    /* DestinationButton → onClick 으로 호출 */
    public void SelectIndex(int idx)
    {
        selectedIdx = idx;
        manager.SelectTarget(idx);
    }
}
