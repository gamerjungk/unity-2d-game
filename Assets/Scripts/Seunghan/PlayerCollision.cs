using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PlayerCollisionWarning : MonoBehaviour
{
    public Text warningText;

    private float cooldown = 1f;        // 경고 표시 쿨타임 1초
    private float lastWarningTime = -10f; // 마지막으로 경고를 띄운 시간 (연속으로 메시지를 띄우지 않기 위해 구분)
    private int pedestrianCollisionCount = 0;
    private float lastPedestrianCollisionTime = -999f;
    private float pedestrianCollisionCooldown = 3f;
    private int pedestrianCollisionLimit = 2;

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        string currentScene = SceneManager.GetActiveScene().name;

        // 보행자와 충돌
        if (other.CompareTag("Pedestrian"))
        {

            if (currentScene == "Tutorial")
            {
                int reward = 2000 * (pedestrianCollisionCount + 1); // 카운트는 증가 안 하지만 보상은 누적 기준
                GameDataManager.Instance.SubMoney(reward);
                Debug.Log("[Tutorial] 돈만 차감: " + reward);
                return;
            }

            if (Time.time - lastPedestrianCollisionTime >= pedestrianCollisionCooldown)
            {
                pedestrianCollisionCount++;
                lastPedestrianCollisionTime = Time.time;

                Debug.Log($"[충돌] 보행자 충돌! 현재 충돌 횟수: {pedestrianCollisionCount}/{pedestrianCollisionLimit}");
                ShowWarning($"보행자 충돌! ({pedestrianCollisionCount}/{pedestrianCollisionLimit})");

                int reward = 5000 * pedestrianCollisionCount;
                GameDataManager.Instance.SubMoney(reward);
                
                
                if (pedestrianCollisionCount >= pedestrianCollisionLimit)
                {
                    if (GameManager.inst != null)
                    {
                        Debug.Log("보행자 충돌 횟수 초과. 라운드 종료!");
                        GameManager.inst.RoundOver();
                    }
                }
            }
            else
            {
                Debug.Log("[충돌] 쿨타임으로 인해 보행자 충돌 무시됨");
            }
        }

        // 嫄대Ъ怨?異⑸룎
        else if (other.CompareTag("Building"))
        {
            ShowWarning("건물과 충돌했습니다!");
            Debug.Log("[충돌] 건물과 충돌 발생");

            if (GameManager.inst != null)
            {
                GameManager.inst.RoundOver();
            }
        }

        // 李⑤웾怨?異⑸룎
        else if (other.CompareTag("Car"))
        {
            ShowWarning("다른 차량과 충돌했습니다!");
            Debug.Log("[충돌] 차량과 충돌 발생");

            if (GameManager.inst != null)
            {
                GameManager.inst.RoundOver();
            }
        }
    }

    void ShowWarning(string message)
    {
        lastWarningTime = Time.time;  // 마지막 메시지 출력 시간 기록
        Debug.Log(message);

        if (warningText != null)
        {
            warningText.text = message;
        }
    }
}

/*
    플레이어가 충돌했을 때 경고 메시지를 출력하는 스크립트

    충돌 대상에 따라 각각 다른 경고 메시지를 출력
    - 보행자 충돌 시 경고 및 보상 차감, 최대 2회까지 허용
    - 보행자 충돌이 일정 횟수를 넘으면 라운드 종료
    - 건물 또는 차량과 충돌 시 즉시 라운드 종료
    - 쿨타임을 설정하여 너무 자주 충돌하지 않도록 방지
*/