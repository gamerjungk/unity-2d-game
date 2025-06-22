using UnityEngine;
using Gley.TrafficSystem;

public class TurnBarTrafficSync_M : MonoBehaviour
{
    // 인스펙터에서 할당할 TurnManager 참조
    [SerializeField] TurnManager turnManager;
    // 이전 프레임의 isMidTurn 상태 저장 변수
    bool lastMidTurn;

    void Awake()
    {
        // Inspector에 할당 안 되어있으면 찾아서 가져오기
        if (turnManager == null)
            turnManager = Object.FindFirstObjectByType<TurnManager>();
    }

    // 초기화 시점에 TurnManager의 상태를 확인하여 차량 움직임을 설정
    void Start()
    {
        // 시작 시점 동기화
        lastMidTurn = turnManager.isMidTurn;
        // 턴이 진행 중이 아니면 차량 정지, 진행 중이면 재생
        ApplyPause(!lastMidTurn);
    }

    void Update()
    {
        // 매 프레임 현재 턴 진행 여부 체크
        bool nowMidTurn = turnManager.isMidTurn;

        // 상태가 이전과 달라졌으면
        if (nowMidTurn != lastMidTurn)
        {
            // 상태가 바뀌었으면 ApplyPause 호출
            ApplyPause(!nowMidTurn);
            lastMidTurn = nowMidTurn; // lastMidTurn 갱신
        }
    }

    // 차량 일시정지 상태 설정 함수
    void ApplyPause(bool shouldPause)
    {
        Debug.Log($"[TurnBarSync] SetPaused({shouldPause})");
        // TrafficPauseManager를 호출해 차량 정지/재생
        TrafficPauseManager_M.SetPaused(shouldPause);
    }
}

/*
    - TurnManager의 isMidTurn 상태에 따라 Gley Traffic System의 차량 움직임을 일시 정지하거나 재개
    - TurnManager가 중간 회전 상태일 때 차량 움직임을 멈추고, 그렇지 않을 때 차량 움직임을 재개
    - TurnManager가 없을 경우 자동으로 찾아서 사용
*/