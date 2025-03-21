using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public GameObject gameoverText;
    public TMP_Text turnreport;
    public TMP_Text crushreport;

    private bool isGameover;
    private float turn;
    private int turndiv = 5;

    void Start(){
        turn = 0;
        isGameover = false;
        gameoverText.SetActive(false);
        if (turnreport == null) {
            Debug.LogError("turnreport가 Inspector에서 할당되지 않았습니다.");
        }
        if (gameoverText == null) {
            Debug.LogError("gameoverText가 Inspector에서 할당되지 않았습니다.");
        }
    }

    void Update(){
        if(!isGameover){
            turn += Time.deltaTime / turndiv;
            if (turnreport != null) {
                turnreport.text = "Turn: " + (int) turn;
            }
        }
        else{
            if(Input.GetKeyDown(KeyCode.R)){
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    public void EndGame(){
        isGameover = true;
        gameoverText.SetActive(true);
        if (gameoverText != null) {
            gameoverText.SetActive(true);
        } else {
            Debug.LogError("gameoverText가 Inspector에서 할당되지 않았습니다.");
        }
    }
}
