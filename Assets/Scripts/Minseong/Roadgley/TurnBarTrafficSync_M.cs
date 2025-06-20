using UnityEngine;
using Gley.TrafficSystem;

public class TurnBarTrafficSync_M : MonoBehaviour
{
    [SerializeField] TurnManager turnManager;
    bool lastMidTurn;

    void Awake()
    {
        // Inspector에 할당 안 되어있으면 찾아서 가져오기
        if (turnManager == null)
            turnManager = Object.FindFirstObjectByType<TurnManager>();
    }

    void Start()
    {
        // 시작 시점 동기화
        lastMidTurn = turnManager.isMidTurn;
        ApplyPause(!lastMidTurn);
    }

    void Update()
    {
        // 매 프레임 현재 턴 진행 여부 체크
        bool nowMidTurn = turnManager.isMidTurn;
        if (nowMidTurn != lastMidTurn)
        {
            // 상태가 바뀌었으면 ApplyPause 호출
            ApplyPause(!nowMidTurn);
            lastMidTurn = nowMidTurn;
        }
    }

    void ApplyPause(bool shouldPause)
    {
        Debug.Log($"[TurnBarSync] SetPaused({shouldPause})");
        TrafficPauseManager_M.SetPaused(shouldPause);
    }
}
