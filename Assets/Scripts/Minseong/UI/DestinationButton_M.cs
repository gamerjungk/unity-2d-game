using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DestinationButton_M : MonoBehaviour
{
    [SerializeField] TMP_Text label;

    // ���� �߰�(3��)
    [Header("Text Colors")]
    [SerializeField] Color pickupColor = Color.black;     // �⺻(�Ⱦ���) ��
    [SerializeField] Color deliveryColor = new(0.12f, 0.55f, 1f); // ����� ��

    int index;
    DestinationUI_M ui;

    /* ���������������������������������������������������������������������������������������������� */
    /* �ʱ�ȭ                                          */
    /* ���������������������������������������������������������������������������������������������� */
    public void Init(int idx, DestinationUI_M parent)
    {
        index = idx;
        ui = parent;
        GetComponent<Button>()
            .onClick.AddListener(() => ui.SelectIndex(index));
    }

    public void SetLabel(string text)
    {
        label.text = text;
    }

    // �Ⱦ�/��޿� ���� �� ���� �߰�
    public void SetAsPickup() => label.color = pickupColor;
    public void SetAsDelivery() => label.color = deliveryColor;

}

/*
    - 목적지 버튼의 UI 요소를 관리하는 스크립트
    - 버튼 클릭 시 DestinationUI_M의 SelectIndex 메서드를 호출하여 선택된 인덱스를 전달
    - 목적지의 종류에 따라 텍스트 색상을 변경하는 기능 포함
*/