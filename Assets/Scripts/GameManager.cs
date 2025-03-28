using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager inst;         // 외부에서 GameManager 함수에 접근하고 싶을 때 GameManager.inst.function() 형식으로 접근하면 됩니다.
    public PoolManager pool;                // 외부에서 PoolManager 함수에 접근하고 싶을 때 GameManager.inst.pool.function() 형식으로 접근하면 됩니다.
    public Player player;                   // 외부에서 Player에 접근하고 싶을 때 GameManager.inst.player.function() 형식으로 접근하면 됩니다.
    public TurnManager turnManager;
    public UIManager uiManager;
    private void Awake()
    {
        inst = this;
    }

    void Start(){
    }

    void Update(){
    }

    public void Stop()      // 게임 배속을 0배속으로 설정. 즉 게임 멈추는 함수입니다.
    {
        Time.timeScale = 0;
    }
    
    public void Resume()    // 게임 배속을 1배속으로 설정. 즉 게임 재개하는 함수입니다.
    {
        Time.timeScale = 1;
    }
}
