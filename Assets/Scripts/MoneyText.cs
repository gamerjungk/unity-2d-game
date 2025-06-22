using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MoneyDisplay : MonoBehaviour
{
    [Tooltip("표시할 TMP(Text) 오브젝트. 비워 두면 자동으로 GetComponent")]
    [SerializeField] private TextMeshProUGUI moneyText;

    private int cachedMoney = int.MinValue;   // 마지막으로 출력한 금액

    void Awake()
    {
        // Inspector에서 안 넣었으면 스스로 찾아 채움
        if (moneyText == null)
            moneyText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (GameDataManager.Instance == null || GameDataManager.Instance.data == null)
            return;

        int currentMoney = GameDataManager.Instance.data.money;

        // 값이 바뀌었을 때만 텍스트 갱신 → GC / 문자열 할당 최소화
        if (currentMoney != cachedMoney)
        {
            cachedMoney = currentMoney; 
            moneyText.text = $"Money : {currentMoney}";
        }

        if (SceneManager.GetActiveScene().name == "Tutorial" && currentMoney >= 50000)
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}

//GameDataManaager.Instance.data.money 값이 변경되면 화면의 텍스트에서 그값만큼 더하거나 빼는것을 실시간으로 확인