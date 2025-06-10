using UnityEngine;
using UnityEngine.UI;

public class PlayerCollisionWarning : MonoBehaviour
{
    public Text warningText;

    private float cooldown = 1f;        // 쿨타임 1초
    private float lastWarningTime = -10f;  // 마지막 경고 시간 초기값 (충돌 직후에도 바로 메시지 뜰 수 있도록 충분히 과거로)

    private int pedestrianCollisionCount = 0;
    private float lastPedestrianCollisionTime = -999f;
    private float pedestrianCollisionCooldown = 3f;
    private int pedestrianCollisionLimit = 2;

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;

        // 보행자와 충돌
        if (other.CompareTag("Pedestrian"))
        {
            if (Time.time - lastPedestrianCollisionTime >= pedestrianCollisionCooldown)
            {
                pedestrianCollisionCount++;
                lastPedestrianCollisionTime = Time.time;

                Debug.Log($"[충돌] 보행자와 충돌! 현재 충돌 횟수: {pedestrianCollisionCount}/{pedestrianCollisionLimit}");
                ShowWarning($"보행자와 충돌! ({pedestrianCollisionCount}/{pedestrianCollisionLimit})");

                if (pedestrianCollisionCount >= pedestrianCollisionLimit)
                {
                    if (GameManager.inst != null)
                    {
                        Debug.Log("보행자 충돌 한도 초과. 라운드 종료!");
                        GameManager.inst.RoundOver();
                    }
                }
            }
            else
            {
                Debug.Log("[충돌] 쿨타임 중이므로 보행자 충돌 카운트 증가 없음");
            }
        }

        // 건물과 충돌
        else if (other.CompareTag("Building"))
        {
            ShowWarning("건물과 충돌했습니다!");
            Debug.Log("[충돌] 건물과 충돌 발생");

            if (GameManager.inst != null)
            {
                GameManager.inst.RoundOver();
            }
        }

        // 차량과 충돌
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
        lastWarningTime = Time.time;  // 마지막 메시지 발생 시간 업데이트
        Debug.Log(message);

        if (warningText != null)
        {
            warningText.text = message;
        }
    }
}
