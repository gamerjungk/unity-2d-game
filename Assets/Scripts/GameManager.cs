using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 외부에서 특정 클래스에 접근할때 사용할 변수
    public static GameManager inst;         
    public PoolManager pool;                
    public Player player;                  
    public TurnManager turnManager;
    public UIManager uiManager;
    public static float fuel = 70;
    //public static int gold = 100;
    //public static int money = 10000;

    private void Awake()
    {
        if (inst != null && inst != this)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }
        inst = this;
        DontDestroyOnLoad(gameObject); // 씬 전환에도 살아있게
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // UIManager 자동 연결
        uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null) uiManager.Init();

        // 다른 매니저들도 필요시 자동 연결
        turnManager = FindFirstObjectByType<TurnManager>();
        player = FindFirstObjectByType<Player>();
        pool = FindFirstObjectByType<PoolManager>();
    }
        
    void Start()
    {

    }

    void Update()
    {
        
    }

    // 시간 정지 함수
    public void Stop()
    {
        Time.timeScale = 0;
    }

    // 시간 재개 함수
    public void Resume()
    {
        Time.timeScale = 1;
    }

    // 라운드 종료 처리 함수
    public void RoundOver()
    {
        Time.timeScale = 1;

        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.data.turn--;

            if (GameDataManager.Instance.data.turn < 0)
                GameDataManager.Instance.data.turn = 0;

            Debug.Log($"턴 감소! 남은 턴: {GameDataManager.Instance.data.turn}");

            // 턴 0일 경우 납부 시도 → 실패 시 조기 종료
            if (GameDataManager.Instance.data.turn == 0)
            {
                Debug.Log("💰 턴 종료 - 납부 시도 중");

                bool success = GameDataManager.Instance.TryPay();

                if (!success)
                {
                    Debug.Log("납부 실패 - 게임 오버로 전환");
                    GameOver(); // 🚨 게임 오버 처리
                    return;     // ⛔ 이후 씬 전환 방지
                }
                else
                {
                    Debug.Log("납부 성공 - 다음 라운드로 이동");
                    // (선택) GameDataManager.Instance.data.turn = 5;
                }
            }

            GameDataManager.Instance.Save();
        }

        LoadSceneManager.Instance.ChangeScene("Shop 2"); // ← 납부할 돈이 충분하면 Shop 2로 이동.
    }

    // 게임 오버 처리 함수. 게임 오버 화면으로 전환
    public void GameOver()
    {
        Time.timeScale = 1; // 혹시 멈춰있을 수도 있으니 복원
        LoadSceneManager.Instance.ChangeScene("GameOverScene");
    }

}
