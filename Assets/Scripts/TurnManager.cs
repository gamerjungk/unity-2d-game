using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    float updateTime = 0f, turnTime = 5f;       // updateTime 시간측정 변수, turnTiem 한 턴 시간 설정
    public int curTurn = 50;                    // 현재 턴
    public bool isMidTurn = false;              // 진행 턴 유무
    public CarObj car;          //테스트용 객체
    void Start()
    {
        
    }
    void Update()
    {
        updateTime += Time.deltaTime;
        if (isMidTurn)                          // 진행턴이라면
        {
            if (updateTime >= turnTime) 
            {
                GameManager.inst.uiManager.uiImages[3].fillAmount = 1f;     // 라운드 프로그레스바 진행
                curTurn--;
                isMidTurn = false;
                car.MoveRandomly();
            }
            else
            {
                GameManager.inst.uiManager.uiImages[3].fillAmount = 1f - (Mathf.Lerp(0, 100, updateTime / turnTime) / 100);
            }
        }
    }

    public void midTurn()
    {
        updateTime = 0;
        isMidTurn = true;
    }

}
