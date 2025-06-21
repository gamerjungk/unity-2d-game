using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MoneyTrigger : MonoBehaviour
{
    bool rewarded;
  
   

    void OnTriggerEnter(Collider other)
    {
        if ( rewarded || !other.CompareTag("Player")) return;

        rewarded = true;                                   // 중복 방지
        DestinationManager.Instance.ArrivedCurrentTarget();// DestinationManage에 ArrivedCurrent.Target 호출하여 플레이어가 목적지 오브젝트랑 충돌하면 보상

    }

    public void ResetTrigger() => rewarded = false;        // DestinationManager 에서 호출
}

//아이디어 변경으로 대부분의 기능이 DestinationManger에  ArrivedCurrentTarget 으로 옮겨짐
//코드 꼬일까봐 삭제안하고 목적지 트리거에만 사용