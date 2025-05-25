using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class PerformanceItemSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("Front (기본 상태)")]
    public GameObject frontFace;
    public Image itemImage;
    public Button actionButton;
    public TextMeshProUGUI actionButtonText;
    public Button useButton; // ✅ 소모성 아이템 전용 사용 버튼

    [Header("Back (상세 정보 상태)")]
    public GameObject backFace;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI priceText;
    public Slider speedSlider;
    public Slider capacitySlider;
    public Slider efficiencySlider;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI capacityText;
    public TextMeshProUGUI efficiencyText;
    public TextMeshProUGUI descriptionText;
    public Button actionButton_Back;
    public TextMeshProUGUI actionButtonText_Back;

    [Header("Animation Settings")]
    [SerializeField] private float gaugeDuration = 1.0f;

    private PerformanceItemSO itemData;
    private bool isFlipped = false;
    private bool isFlipping = false;

    private Coroutine speedCoroutine;
    private Coroutine capacityCoroutine;
    private Coroutine efficiencyCoroutine;

    public void Setup(PerformanceItemSO data)
    {
        itemData = data;

        itemImage.sprite = itemData.image;
        UpdateActionButton();

        itemNameText.text = itemData.DisplayName;
        priceText.text = $"{itemData.price}G";
        descriptionText.text = itemData.DisplayDescription;

        useButton.gameObject.SetActive(false); // 기본은 숨김

        isFlipped = false;
        frontFace.SetActive(true);
        backFace.SetActive(false);
        transform.localRotation = Quaternion.identity;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isFlipping) return;
        StartCoroutine(FlipCard());
    }

    private IEnumerator FlipCard()
    {
        isFlipping = true;

        float duration = 0.15f;
        float elapsed = 0f;
        Quaternion midRot = Quaternion.Euler(0f, 90f, 0f);
        Quaternion endRot = isFlipped ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f);
        Quaternion startRot = transform.localRotation;

        while (elapsed < duration)
        {
            transform.localRotation = Quaternion.Lerp(startRot, midRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = midRot;

        isFlipped = !isFlipped;
        frontFace.SetActive(!isFlipped);
        backFace.SetActive(isFlipped);

        if (isFlipped)
        {
            backFace.transform.localRotation = Quaternion.Euler(0, 180f, 0);
            speedSlider.value = 0;
            capacitySlider.value = 0;
            efficiencySlider.value = 0;
            speedText.text = "0";
            capacityText.text = "0";
            efficiencyText.text = "0";

            if (speedCoroutine != null) StopCoroutine(speedCoroutine);
            if (capacityCoroutine != null) StopCoroutine(capacityCoroutine);
            if (efficiencyCoroutine != null) StopCoroutine(efficiencyCoroutine);

            speedCoroutine = StartCoroutine(AnimateSlider(speedSlider, speedText, itemData.speed));
            capacityCoroutine = StartCoroutine(AnimateSlider(capacitySlider, capacityText, itemData.capacity));
            efficiencyCoroutine = StartCoroutine(AnimateSlider(efficiencySlider, efficiencyText, itemData.efficiency));
        }
        else
        {
            backFace.transform.localRotation = Quaternion.identity;
        }

        elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localRotation = Quaternion.Lerp(midRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = endRot;
        isFlipping = false;
    }

    private IEnumerator AnimateSlider(Slider slider, TextMeshProUGUI valueText, float target)
    {
        float elapsed = 0f;

        while (elapsed < gaugeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / gaugeDuration);
            float value = Mathf.Lerp(0f, target, t);
            slider.value = value;
            valueText.text = value.ToString("F1");
            yield return null;
        }

        slider.value = target;
        valueText.text = target.ToString("F1");
    }

    private void SetButtonState(Button button, TextMeshProUGUI text, UnityEngine.Events.UnityAction callback, string label, bool interactable)
    {
        text.text = label;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(callback);
        button.interactable = interactable;
    }

    public void UpdateActionButton()
    {
        bool isOwned = PerformanceInventoryManager.Instance.IsOwned(itemData);
        bool isEquipped = PerformanceInventoryManager.Instance.IsEquipped(itemData.category, itemData);

        if (!isOwned)
        {
            SetButtonState(actionButton, actionButtonText, () => PerformanceShopManager.Instance.BuySelectedItem(itemData), "구매", true);
            SetButtonState(actionButton_Back, actionButtonText_Back, () => PerformanceShopManager.Instance.BuySelectedItem(itemData), "구매", true);
        }
        else
        {
            string label = isEquipped ? "장착중" : "장착";
            bool interactable = !isEquipped;

            SetButtonState(actionButton, actionButtonText, () => PerformanceShopManager.Instance.EquipSelectedItem(itemData), label, interactable);
            SetButtonState(actionButton_Back, actionButtonText_Back, () => PerformanceShopManager.Instance.EquipSelectedItem(itemData), label, interactable);
        }
    }

    public void EnableUseButton(UnityEngine.Events.UnityAction onClick)
    {
        useButton.gameObject.SetActive(true);
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(onClick);
    }

    public void Refresh()
    {
        UpdateActionButton();
    }
}
