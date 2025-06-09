using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class TutorialStep
{
    public string message;
    public Vector2 panelPosition;
    public Vector2 arrowPosition;
    public bool useArrow;
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

    private int currentStep = 0;
    private Vector2 originalArrowPos;
    private float arrowAnimTime = 0f;
    public float arrowMoveAmplitude = 10f;
    public float arrowMoveSpeed = 2f;

    void Start()
    {
        nextButton.onClick.AddListener(NextStep);
        ShowStep();
    }
  
    void Update()
    {

        // 화살표 움직임 애니메이션
        if (arrowRect != null && arrowRect.gameObject.activeSelf)
        {
            arrowAnimTime += Time.unscaledDeltaTime; // Time.timeScale = 0이어도 작동
            float offsetY = Mathf.Sin(arrowAnimTime * arrowMoveSpeed) * arrowMoveAmplitude;
            arrowRect.anchoredPosition = originalArrowPos + new Vector2(0f, offsetY);
        }
    }
    void ShowStep()
    {
        if (currentStep >= tutorialSteps.Count)
        {
            tutorialPanel.SetActive(false);
            arrowRect.gameObject.SetActive(false);
            Time.timeScale = 1f;
            return;
        }

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
            if (arrowRect != null)
                arrowRect.gameObject.SetActive(false);
        }
    }

    public void NextStep()
    {
        currentStep++;
        ShowStep();
    }
}