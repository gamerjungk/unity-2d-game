using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DestinationUI_M : MonoBehaviour
{
    /* ──────────────── 인스펙터 ──────────────── */
    [Header("External refs")] // 헤더 표시
    [SerializeField] DestinationManager manager; // DestinationSystem 밑 Manager
    [SerializeField] RectTransform panel; // 버튼들이 배치될 UI패널의 DestPanel
    [SerializeField] DestinationButton_M buttonPrefab; //버튼 프리팹 참조

    /* ──────────────── 내부 상태 ──────────────── */
    readonly List<DestinationButton_M> buttons = new(); // 생성된 버튼 인스턴스를 저장할 리스트  
    CanvasGroup cg; // 패널의 CanvasGroup (터치 블록 및 투명도 조절용)  
    int selectedIdx = -1; // 현재 선택된 버튼 인덱스 (초기는 없음)  
    bool[] isPickup; // 각 버튼이 픽업 단계인지 배달 단계인지 저장  
    public static DestinationUI_M Instance { get; private set; } // 싱글턴 인스턴스  

    void Awake()
    {
        Instance = this; // 싱글턴 인스턴스로 설정

        // panel의 CanvasGroup 컴포넌트를 가져오거나 없으면 추가
        cg = panel.GetComponent<CanvasGroup>() ?? panel.gameObject.AddComponent<CanvasGroup>();

        cg.interactable = true; // 터치 허용
        cg.blocksRaycasts = true; // Raycast 차단 허용 설정

        // DestinationManager가 목표 도착 알림 이벤트 발생 시 ArrivedAt 메서드 호출 
        DestinationManager.OnArrivedTarget += ArrivedAt;
    }

    // 외부에서 픽업.배달 상태 조회 함수
    public bool GetPickupState(int idx)
    {
        // 배열이 초기화되고, 인덱스 범위 내이며, 해당 값이 true(픽업)인지 반환
        return isPickup != null && idx >= 0 && idx < isPickup.Length && isPickup[idx];
    }

    void Start()
    {
        int n = manager.Markers.Length; // DestinationManager에 설정된 마커 개수
        isPickup = Enumerable.Repeat(true, n).ToArray(); // 전부 픽업 상태로 시작

        // 버튼 프리팹을 n개 생성하여 초기 설정
        for (int i = 0; i < n; ++i)
        {
            var btn = Instantiate(buttonPrefab, panel); // 패널 자식으로 버튼 인스턴스화  
            btn.Init(i, this); // 버튼에 인덱스와 부모 UI 전달  
            buttons.Add(btn); // 리스트에 저장  
            btn.SetAsPickup(); // 초기 텍스트 색상을 픽업 색으로 설정
        }
    }

    void Update()
    {
        Vector3 playerPos = manager.Player.position; // 플레이어 현재 위치로 설정

        // 모든 버튼에 대해
        for (int i = 0; i < buttons.Count; ++i)
        {
            // 플레이어와 마커 간 거리 계산
            float dist = Vector3.Distance(playerPos, manager.Markers[i].position);
            string prefix = isPickup[i] ? "픽업지" : "배달지"; // 픽업/배달에 따라 접두어 설정

            // 버튼 라벨에 "픽업지1  12 m" 형식으로 거리 표시
            buttons[i].SetLabel($"{prefix}{i + 1}  {dist:0} m");

            // 선택된 버튼만 완전 불투명, 나머지는 반투명으로 설정
            Image img = buttons[i].GetComponent<Image>();
            img.color = new Color(1, 1, 1, i == selectedIdx ? 1f : 0.55f);

            // 픽업/배달 상태에 따라 텍스트 색상 업데이트
            if (isPickup[i]) buttons[i].SetAsPickup();
            else buttons[i].SetAsDelivery();
        }
    }

    /* 버튼 클릭 시 호출 */
    public void SelectIndex(int idx)
    {
        selectedIdx = idx; // 선택된 인덱스 저장
        manager.SelectTarget(idx); // DestinationManager에 목표 변경 요청
    }

    /* DestinationManager가 도착 이벤트 발생 시 호출 */
    public void ArrivedAt(int idx)
    {
        // 유효하지 않은 인덱스면 무시
        if (idx < 0 || idx >= isPickup.Length) return;

        isPickup[idx] = !isPickup[idx]; // 픽업 ↔ 배달 상태 토글

        // 상태 토글 시 즉시 글자 색 반영
        if (isPickup[idx]) buttons[idx].SetAsPickup();
        else buttons[idx].SetAsDelivery();
    }
}
