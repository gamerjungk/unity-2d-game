using UnityEngine;
using TMPro;

public class MoneyDisplay : MonoBehaviour
{
    public TextMeshProUGUI moneyText;

    void Update()
    {
        moneyText.text = "Money: " + GameManager.money.ToString();
    }
}