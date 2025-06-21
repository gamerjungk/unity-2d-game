using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    float updateTime = 0f, turnTime = 5f;       
    public int curTurn = 50;                    
    public bool isMidTurn = false;              
    void Start()
    {
        
    }
    void Update()
    {
        updateTime += Time.deltaTime;
        if (isMidTurn)                          
        {
            // 턴이 진행되고 있다면 턴 프로그레스바를 turnTime동안 진행
            if (updateTime >= turnTime)
            {
                GameManager.inst.uiManager.uiImages[8].fillAmount = 1f;
                curTurn--;
                isMidTurn = false;

                // 전체 턴이 전부 진행되면 징수 화면으로 전환
                if (curTurn <= 0)
                {
                    LoadSceneManager.Instance.ChangeScene("Shop 2");
                }
            }
            // 한턴이 소모되면 프로그레스바 복구
            else
            {
                GameManager.inst.uiManager.uiImages[8].fillAmount = 1f - (Mathf.Lerp(0, 100, updateTime / turnTime) / 100);
            }
        }
    }

    // 턴 진행 함수
    public void midTurn()
    {
        if (!isMidTurn)
        {
            updateTime = 0;
            isMidTurn = true;
        }
    }

}
