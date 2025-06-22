using UnityEngine;
using UnityEngine.UI;

public class MinimapUIScaler : MonoBehaviour
{
    public RectTransform minimapUI;

    void Start()
    {
        float screenWidth = Screen.width;
        float size = screenWidth * 0.3f; // ȭ�� �ʺ��� 30%
        minimapUI.sizeDelta = new Vector2(size, size);
    }
}