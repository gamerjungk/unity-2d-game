using UnityEngine;
using UnityEngine.UI;

public class MinimapUIScaler : MonoBehaviour
{
    // 미니맵 UI에 연결할 변수 생성
    public RectTransform minimapUI;

    void Start()
    {
        // 현재 화면의 너비 호출
        float screenWidth = Screen.width;
        // 미니맵 사이즈 크기 = 화면 너비의 30%
        float size = screenWidth * 0.3f;
        // 미니맵 UI 가로, 세로 크기 설정
        minimapUI.sizeDelta = new Vector2(size, size);
    }
}


