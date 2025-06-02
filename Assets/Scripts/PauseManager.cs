using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    public GameObject pausePanel;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        pausePanel.SetActive(false);
    }

    public void TogglePause(bool isPaused)
    {
        Time.timeScale = isPaused ? 0f : 1f;
        pausePanel.SetActive(isPaused);
    }

    public void OnPauseButtonClicked()
    {
        TogglePause(true);
    }

    public void OnResumeButtonClicked()
    {
        TogglePause(false);
    }

    // 모바일 백그라운드 감지
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && Time.timeScale > 0f)
        {
            TogglePause(true);
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && Time.timeScale > 0f)
        {
            TogglePause(true);
        }
    }
}
