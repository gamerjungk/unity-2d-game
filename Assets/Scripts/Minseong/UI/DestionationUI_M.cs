using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DestinationUI_M : MonoBehaviour
{
    [Header("External refs")]
    [SerializeField] DestinationManager manager;
    [SerializeField] RectTransform panel;
    [SerializeField] DestinationButton_M buttonPrefab;

    readonly List<DestinationButton_M> buttons = new();
    CanvasGroup cg;
    int selectedIdx = -1;
    bool[] isPickup;                               // true = 픽업 단계

    /* ─────────────────────────────── */
    void Awake()
    {
        cg = panel.GetComponent<CanvasGroup>() ??
             panel.gameObject.AddComponent<CanvasGroup>();
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    void Start()
    {
        int n = manager.Markers.Length;
        isPickup = Enumerable.Repeat(true, n).ToArray();

        for (int i = 0; i < n; ++i)
        {
            var btn = Instantiate(buttonPrefab, panel);
            btn.Init(i, this);
            buttons.Add(btn);
        }
    }

    void Update()
    {
        Vector3 playerPos = manager.Player.position;

        for (int i = 0; i < buttons.Count; ++i)
        {
            float dist = Vector3.Distance(playerPos, manager.Markers[i].position);
            string stage = isPickup[i] ? "픽업지" : "배달지";

            buttons[i].SetLabel($"{stage}{i + 1}  {dist:0} m");
            buttons[i].SetStage(isPickup[i]);

            Image img = buttons[i].GetComponent<Image>();
            img.color = new Color(1, 1, 1, (i == selectedIdx) ? 1f : 0.55f);
        }
    }

    /* 버튼 onClick → 호출 */
    public void SelectIndex(int idx)
    {
        selectedIdx = idx;
        manager.SelectTarget(idx);
    }

    /* Manager → 도착 알림 */
    public void ArrivedAt(int idx)
    {
        if ((uint)idx >= (uint)isPickup.Length) return;

        isPickup[idx] = !isPickup[idx];          // 단계 토글
        buttons[idx].SetStage(isPickup[idx]);    // 글자 색 즉시 반영
    }

    /* Manager 가 현재 단계 체크할 때 사용 */
    public bool IsPickup(int idx) =>
        (uint)idx < (uint)isPickup.Length && isPickup[idx];
}
