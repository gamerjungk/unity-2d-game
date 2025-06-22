using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class TutorialStep
{
    public string message; // 튜토리얼에서 표시할 메시지
    public Vector2 panelPosition; // 튜토리얼 패널의 위치
    public Vector2 arrowPosition; // 화살표 위치
    public bool useArrow;  // 이 단계에서 화살표를 사용할지 여부
}

public class TutorialManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialText;
    public Button nextButton;

    [Header("Arrow")]
    public RectTransform arrowRect;

    [Header("Tutorial Data")]
    public List<TutorialStep> tutorialSteps;

    private int currentStep = 0; // 현재 튜토리얼 단계 인덱스
    private Vector2 originalArrowPos;  // 화살표의 원래 위치 
    private float arrowAnimTime = 0f;
    public float arrowMoveAmplitude = 10f;   // 화살표 이동 범위
    public float arrowMoveSpeed = 2f;

    void Start()
    {
        // 버튼 클릭 시 NextStep 메서드 호출하며 다음 텍스트 출력
        nextButton.onClick.AddListener(NextStep);
        ShowStep();
    }

    void Update()
    {

        // 화살표를 위아래로 움직여서 강조효과를 높이기 위해 사용
        if (arrowRect != null && arrowRect.gameObject.activeSelf)
        {
            arrowAnimTime += Time.unscaledDeltaTime; // Time.timeScale = 0이어도 작동
            float offsetY = Mathf.Sin(arrowAnimTime * arrowMoveSpeed) * arrowMoveAmplitude;
            arrowRect.anchoredPosition = originalArrowPos + new Vector2(0f, offsetY);
        }
    }

    // 현재 단계 튜토리얼 내용을 화면에 표시
    void ShowStep()
    {
        // 튜토리얼이 끝났으면 UI 비활성화 
        if (currentStep >= tutorialSteps.Count)
        {
            tutorialPanel.SetActive(false);
            arrowRect.gameObject.SetActive(false);
            Time.timeScale = 1f;
            return;
        }
        // 현재 단계 튜토리얼 데이터 가져오기
        TutorialStep step = tutorialSteps[currentStep];
        tutorialText.text = step.message;

        RectTransform panelRect = tutorialPanel.GetComponent<RectTransform>();
        panelRect.anchoredPosition = step.panelPosition;

        if (step.useArrow && arrowRect != null)
        {
            arrowRect.gameObject.SetActive(true);
            arrowRect.anchoredPosition = step.arrowPosition;
            originalArrowPos = step.arrowPosition;
            arrowAnimTime = 0f;
        }
        else
        {
            // 화살표 사용하지 않으면 비활성화
            if (arrowRect != null)
                arrowRect.gameObject.SetActive(false);
        }
    }

    // 다음 튜토리얼 단계로 넘어감
    public void NextStep()
    {
        currentStep++;
        ShowStep();
    }
}