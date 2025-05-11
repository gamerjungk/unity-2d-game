using UnityEngine;
using UnityEngine.UI;

public class MinimapUIScaler : MonoBehaviour
{
    // �̴ϸ� UI�� ������ ���� ����
    public RectTransform minimapUI;

    void Start()
    {
        // ���� ȭ���� �ʺ� ȣ��
        float screenWidth = Screen.width;
        // �̴ϸ� ������ ũ�� = ȭ�� �ʺ��� 30%
        float size = screenWidth * 0.3f;
        // �̴ϸ� UI ����, ���� ũ�� ����
        minimapUI.sizeDelta = new Vector2(size, size);
    }
}


