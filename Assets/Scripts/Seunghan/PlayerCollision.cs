using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PlayerCollisionWarning : MonoBehaviour
{
    public Text warningText;

    private float cooldown = 1f;        // ��� ǥ�� ��Ÿ�� 1��
    private float lastWarningTime = -10f; // ���������� ��� ��� �ð� (�������� �޽����� ����� �ʱ� ���� ����)
    private int pedestrianCollisionCount = 0;
    private float lastPedestrianCollisionTime = -999f;
    private float pedestrianCollisionCooldown = 3f;
    private int pedestrianCollisionLimit = 2;

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        string currentScene = SceneManager.GetActiveScene().name;

        // �����ڿ� �浹
        if (other.CompareTag("Pedestrian"))
        {

            if (currentScene == "Tutorial")
            {
                int reward = 2000 * (pedestrianCollisionCount + 1); // ī��Ʈ�� ���� �� ������ ������ ���� ����
                GameDataManager.Instance.SubMoney(reward);
                Debug.Log("[Tutorial] ���� ����: " + reward);
                return;
            }

            if (Time.time - lastPedestrianCollisionTime >= pedestrianCollisionCooldown)
            {
                pedestrianCollisionCount++;
                lastPedestrianCollisionTime = Time.time;

                Debug.Log($"[�浹] ������ �浹! ���� �浹 Ƚ��: {pedestrianCollisionCount}/{pedestrianCollisionLimit}");
                ShowWarning($"������ �浹! ({pedestrianCollisionCount}/{pedestrianCollisionLimit})");

                int reward = 5000 * pedestrianCollisionCount;
                GameDataManager.Instance.SubMoney(reward);
                
                
                if (pedestrianCollisionCount >= pedestrianCollisionLimit)
                {
                    if (GameManager.inst != null)
                    {
                        Debug.Log("������ �浹 Ƚ�� �ʰ�. ���� ����!");
                        GameManager.inst.RoundOver();
                    }
                }
            }
            else
            {
                Debug.Log("[�浹] ��Ÿ������ ���� ������ �浹 ���õ�");
            }
        }

        // 건물�?충돌
        else if (other.CompareTag("Building"))
        {
            ShowWarning("�ǹ��� �浹�߽��ϴ�!");
            Debug.Log("[�浹] �ǹ��� �浹 �߻�");

            if (GameManager.inst != null)
            {
                GameManager.inst.RoundOver();
            }
        }

        // 차량�?충돌
        else if (other.CompareTag("Car"))
        {
            ShowWarning("�ٸ� ������ �浹�߽��ϴ�!");
            Debug.Log("[�浹] ������ �浹 �߻�");

            if (GameManager.inst != null)
            {
                GameManager.inst.RoundOver();
            }
        }
    }

    void ShowWarning(string message)
    {
        lastWarningTime = Time.time;  // ������ �޽��� ��� �ð� ���
        Debug.Log(message);

        if (warningText != null)
        {
            warningText.text = message;
        }
    }
}

/*
    �÷��̾ �浹���� �� ��� �޽����� ����ϴ� ��ũ��Ʈ

    �浹 ��� ���� ���� �ٸ� ��� �޽����� ���
    - ������ �浹 �� ��� �� ���� ����, �ִ� 2ȸ���� ���
    - ������ �浹�� ���� Ƚ���� ������ ���� ����
    - �ǹ� �Ǵ� ������ �浹 �� ��� ���� ����
    - ��Ÿ���� �����Ͽ� �ʹ� ���� �浹���� �ʵ��� ����
*/