using UnityEngine;
using TMPro;

public class MoneyDisplay : MonoBehaviour
{
    [Tooltip("ǥ���� TMP(Text) ������Ʈ. ��� �θ� �ڵ����� GetComponent")]
    [SerializeField] private TextMeshProUGUI moneyText;

    private int cachedMoney = int.MinValue;   // ���������� ����� �ݾ�

    void Awake()
    {
        // Inspector���� �� �־����� ������ ã�� ä��
        if (moneyText == null)
            moneyText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (GameDataManager.Instance == null || GameDataManager.Instance.data == null)
            return;

        int currentMoney = GameDataManager.Instance.data.money;

        // ���� �ٲ���� ���� �ؽ�Ʈ ���� �� GC / ���ڿ� �Ҵ� �ּ�ȭ
        if (currentMoney != cachedMoney)
        {
            cachedMoney = currentMoney; 
            moneyText.text = $"Money : {currentMoney}";
        }
    }
}

//GameDataManaager.Instance.data.money ���� ����Ǹ� ȭ���� �ؽ�Ʈ���� �װ���ŭ ���ϰų� ���°��� �ǽð����� Ȯ��