using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MoneyTrigger : MonoBehaviour
{
    bool rewarded;
  
    void OnTriggerEnter(Collider other)
    {
        if ( rewarded || !other.CompareTag("Player")) return;

        rewarded = true;                                   // �ߺ� ����
        DestinationManager.Instance.ArrivedCurrentTarget();// DestinationManage�� ArrivedCurrent.Target ȣ���Ͽ� �÷��̾ ������ ������Ʈ�� �浹�ϸ� ����

    }

    public void ResetTrigger() => rewarded = false;        // DestinationManager ���� ȣ��
}

//���̵�� �������� ��κ��� ����� DestinationManger��  ArrivedCurrentTarget ���� �Ű���
//�ڵ� ���ϱ�� �������ϰ� ������ Ʈ���ſ��� ���