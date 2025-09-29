using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ShopManagerPerf : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public Transform gridParent;
    public GameObject perfSlotPrefab;      // ItemSlotPerf가 붙은 프리팹
    public PerformanceItemSO[] items;      // 인스펙터에 NavGuidePass 포함

    void OnEnable() { Build(); }
    public void RefreshAll() { UpdateMoneyUI(); Rebuild(); }

    void Build()
    {
        UpdateMoneyUI();
        Rebuild();
    }
    void UpdateMoneyUI()
    {
        moneyText.text = $"{GameDataManager.Instance.data.money}원";
    }
    void Rebuild()
    {
        foreach (Transform c in gridParent) Destroy(c.gameObject);
        foreach (var so in items)
        {
            var go = Instantiate(perfSlotPrefab, gridParent);
            go.GetComponent<ItemSlotPerf>().Setup(so);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(gridParent as RectTransform);
    }

}
