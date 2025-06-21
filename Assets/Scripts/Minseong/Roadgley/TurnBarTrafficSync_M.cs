using UnityEngine;

public class TurnBarTrafficSync_M : MonoBehaviour
{
    [SerializeField] TurnManager turnManager;
    bool lastRunning;

    void Awake()
    {
        if (turnManager == null)
            turnManager = FindObjectOfType<TurnManager>();
    }

    void Start()               // ���� ���� ����ȭ
    {
        lastRunning = turnManager.isMidTurn;
        TrafficPauseManager_M.SetPaused(!lastRunning);
    }

    void Update()
    {
        bool nowRunning = turnManager.isMidTurn;
        if (nowRunning != lastRunning)
        {
            TrafficPauseManager_M.SetPaused(!nowRunning);
            lastRunning = nowRunning;
        }
    }
}

/*
    - TurnManager의 isMidTurn 상태에 따라 Gley Traffic System의 차량 움직임을 일시 정지하거나 재개
    - TurnManager가 중간 회전 상태일 때 차량 움직임을 멈추고, 그렇지 않을 때 차량 움직임을 재개
    - TurnManager가 없을 경우 자동으로 찾아서 사용
*/