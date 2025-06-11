using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DestinationUI_M : MonoBehaviour
{
    /* ──────────────── 인스펙터 ──────────────── */
    [Header("External refs")]
    [SerializeField] DestinationManager manager;      // DestinationSystem 밑 Manager
    [SerializeField] RectTransform panel;        // DestPanel
    [SerializeField] DestinationButton_M buttonPrefab;

    /* ──────────────── 내부 상태 ──────────────── */
    readonly List<DestinationButton_M> buttons = new();
    CanvasGroup cg;
    int selectedIdx = -1;
    bool[] isPickup; // 픽업/배달 상태 저장

    /* ─────────────────────────────────────────── */
    void Awake()
    {
        cg = panel.GetComponent<CanvasGroup>() ?? panel.gameObject.AddComponent<CanvasGroup>();
        cg.interactable = true;    // 터치 허용
        cg.blocksRaycasts = true;

        DestinationManager.OnArrivedTarget += ArrivedAt; // 도착 이벤트
    }

    void Start()
    {
        int n = manager.Markers.Length;
        isPickup = Enumerable.Repeat(true, n).ToArray();     // 전부 픽업 상태로 시작

        for (int i = 0; i < n; ++i)
        {
            var btn = Instantiate(buttonPrefab, panel);
            btn.Init(i, this);
            buttons.Add(btn);
            btn.SetAsPickup();
        }
    }

    void Update()
    {
        Vector3 playerPos = manager.Player.position;

        for (int i = 0; i < buttons.Count; ++i)
        {
            float dist = Vector3.Distance(playerPos, manager.Markers[i].position);
            string prefix = isPickup[i] ? "픽업지" : "배달지";

            buttons[i].SetLabel($"{prefix}{i + 1}  {dist:0} m");

            // 선택된 버튼만 완전 불투명
            Image img = buttons[i].GetComponent<Image>();
            img.color = new Color(1, 1, 1, i == selectedIdx ? 1f : 0.55f);

            // 글자 색 적용 추가
            if (isPickup[i]) buttons[i].SetAsPickup();
            else buttons[i].SetAsDelivery();
        }
    }

    /* ───────── 버튼 onClick → 호출 ───────── */
    public void SelectIndex(int idx)
    {
        selectedIdx = idx;
        manager.SelectTarget(idx);
    }

    /* ───── DestinationManager → 도착 알림 ───── */
    public void ArrivedAt(int idx)
    {
        if (idx < 0 || idx >= isPickup.Length) return;

        isPickup[idx] = !isPickup[idx];      // 픽업 ↔ 배달 상태 토글

        // 글자 색 바로 반영 추가
        if (isPickup[idx]) buttons[idx].SetAsPickup();
        else buttons[idx].SetAsDelivery();
    }
}
