using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    float updateTime = 0f, turnTime = 5f;       // updateTime �ð����� ����, turnTiem �� �� �ð� ����
    public int curTurn = 50;                    // ���� ��
    public bool isMidTurn = false;              // ���� �� ����
    // public CarObj car;          //�׽�Ʈ�� ��ü
    void Start()
    {
        
    }
    void Update()
    {
        updateTime += Time.deltaTime;
        if (isMidTurn)                          // �������̶��
        {
            if (updateTime >= turnTime) 
            {
                GameManager.inst.uiManager.uiImages[8].fillAmount = 1f;     // ���� ���α׷����� ����
                curTurn--;
                isMidTurn = false;

                if (curTurn <= 0)
                {
                    LoadSceneManager.Instance.ChangeScene("Shop 2");
                }
            }
            else
            {
                GameManager.inst.uiManager.uiImages[8].fillAmount = 1f - (Mathf.Lerp(0, 100, updateTime / turnTime) / 100);
            }
        }
    }

    public void midTurn()
    {
        if(!isMidTurn) {
            updateTime = 0;
            isMidTurn = true;
        } 
    }

}
