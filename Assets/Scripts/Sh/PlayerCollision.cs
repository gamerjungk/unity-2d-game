using UnityEngine;
using UnityEngine.UI;

public class PlayerCollisionWarning : MonoBehaviour
{
    public Text warningText;

    private float cooldown = 1f;        // 쿨타임 1초
    private float lastWarningTime = -10f;  // 마지막 경고 시간 초기값 (충돌 직후에도 바로 메시지 뜰 수 있도록 충분히 과거로)

    void Start()
    {
        if (warningText != null)
        {
            warningText.text = "";
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time - lastWarningTime < cooldown)
        {
            // 쿨타임 안 지나면 메시지 무시
            return;
        }

        GameObject other = collision.gameObject;

        if (other.CompareTag("Pedestrian"))
        {
            ShowWarning("위험! 보행자와 충돌했습니다!");
        }
        else if (other.CompareTag("Building"))
        {
            ShowWarning("건물과 충돌했습니다!");
        }
        else if (other.CompareTag("Car"))
        {
            ShowWarning("다른 차량과 충돌했습니다!");
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
