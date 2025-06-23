using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class TutorialStep
{
    public string message; // Ʃ�丮�󿡼� ǥ���� �޽���
    public Vector2 panelPosition; // Ʃ�丮�� �г��� ��ġ
    public Vector2 arrowPosition; // ȭ��ǥ ��ġ
    public bool useArrow;  // �� �ܰ迡�� ȭ��ǥ�� ������� ����
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

    private int currentStep = 0; // ���� Ʃ�丮�� �ܰ� �ε���
    private Vector2 originalArrowPos;  // ȭ��ǥ�� ���� ��ġ 
    private float arrowAnimTime = 0f;
    public float arrowMoveAmplitude = 10f;   // ȭ��ǥ �̵� ����
    public float arrowMoveSpeed = 2f;

    void Start()
    {
        // ��ư Ŭ�� �� NextStep �޼��� ȣ���ϸ� ���� �ؽ�Ʈ ���
        nextButton.onClick.AddListener(NextStep);
        ShowStep();
    }

    void Update()
    {

        // ȭ��ǥ�� ���Ʒ��� �������� ����ȿ���� ���̱� ���� ���
        if (arrowRect != null && arrowRect.gameObject.activeSelf)
        {
            arrowAnimTime += Time.unscaledDeltaTime; // Time.timeScale = 0�̾ �۵�
            float offsetY = Mathf.Sin(arrowAnimTime * arrowMoveSpeed) * arrowMoveAmplitude;
            arrowRect.anchoredPosition = originalArrowPos + new Vector2(0f, offsetY);
        }
    }

    // ���� �ܰ� Ʃ�丮�� ������ ȭ�鿡 ǥ��
    void ShowStep()
    {
        // Ʃ�丮���� �������� UI ��Ȱ��ȭ 
        if (currentStep >= tutorialSteps.Count)
        {
            tutorialPanel.SetActive(false);
            arrowRect.gameObject.SetActive(false);
            Time.timeScale = 1f;
            return;
        }
        // ���� �ܰ� Ʃ�丮�� ������ ��������
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
            // ȭ��ǥ ������� ������ ��Ȱ��ȭ
            if (arrowRect != null)
                arrowRect.gameObject.SetActive(false);
        }
    }

    // ���� Ʃ�丮�� �ܰ�� �Ѿ
    public void NextStep()
    {
        currentStep++;
        ShowStep();
    }
}