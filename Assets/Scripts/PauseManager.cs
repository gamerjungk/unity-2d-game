using UnityEngine;

// 게임의 일시정지 및 재개를 관리하는 매니저 클래스.
// UI 패널 활성화와 함께 Time.timeScale 조작으로 게임 흐름을 제어.
public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }   // 싱글턴 인스턴스
    public GameObject pausePanel;   // 일시정지 시 표시될 UI 패널

    private void Awake()
    {
        // 중복 인스턴스 방지
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {   
        // 게임 시작 시 일시정지 패널 비활성화
        pausePanel.SetActive(false);
    }

    // 게임의 일시정지 상태를 토글하는 함수.
    public void TogglePause(bool isPaused)
    {
        Time.timeScale = isPaused ? 0f : 1f;    // Time.timeScale을 0 또는 1로 변경하여 게임 정지/재개를 구현.
        pausePanel.SetActive(isPaused);
    }


    public void OnPauseButtonClicked()
    {
        TogglePause(true);  // 일시정지 버튼 클릭 시 TogglePause를 호출하여 timeScale을 0으로 만들어 게임 정지.
    }

    public void OnResumeButtonClicked()
    {
        TogglePause(false); // 재개 버튼 클릭 시 다시 TogglePause를 호출하여 timeScale을 1으로 만들어 게임 재개.
    }

    // 모바일 백그라운드 감지
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && Time.timeScale > 0f)
        {
            TogglePause(true);  // 앱이 백그라운드로 전환되면 자동 일시정지 처리
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && Time.timeScale > 0f)
        {
            TogglePause(true);  // 포커스를 잃었을 때도 자동으로 일시정지 처리 (예: 홈버튼 누름 등)
        }
    }
}
