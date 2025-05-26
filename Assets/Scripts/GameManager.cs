using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;         // �ܺο��� GameManager �Լ��� �����ϰ� ���� �� GameManager.inst.function() �������� �����ϸ� �˴ϴ�.
    public PoolManager pool;                // �ܺο��� PoolManager �Լ��� �����ϰ� ���� �� GameManager.inst.pool.function() �������� �����ϸ� �˴ϴ�.
    public Player player;                   // �ܺο��� Player�� �����ϰ� ���� �� GameManager.inst.player.function() �������� �����ϸ� �˴ϴ�.
    public TurnManager turnManager;
    public UIManager uiManager;
    public static float fuel = 70;
    //public static int gold = 100;
    //public static int money = 10000;

    private void Awake()
    {
        inst = this;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public void Stop()      // ���� ����� 0������� ����. �� ���� ���ߴ� �Լ��Դϴ�.
    {
        Time.timeScale = 0;
    }

    public void Resume()    // ���� ����� 1������� ����. �� ���� �簳�ϴ� �Լ��Դϴ�.
    {
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        Time.timeScale = 1; // 혹시 멈춰있을 수도 있으니 복원
        SceneManager.LoadScene("GameOverScene");
    }

}
