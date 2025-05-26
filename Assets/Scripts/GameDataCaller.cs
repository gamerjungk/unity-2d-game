using UnityEngine;

public class GameDataCaller : MonoBehaviour
{
    public void CallReset()
    {
        GameDataManager.Instance?.ResetGameData();
        PerformanceShopManager.Instance?.UpdateMoneyUI();
        PerformanceShopManager.Instance?.UpdateTurnAndPaymentUI();
    }

    public void AddMoney(int amount)
    {
        GameDataManager.Instance?.AddMoney(amount);
    }
}
