using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class PerformanceItemSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("Front (기본 상태)")]
    public GameObject frontFace;                // 카드 앞면 (기본 정보 UI)
    public Image itemImage;                     // 아이템 이미지
    public Button actionButton;                 // 앞면에서의 구매/장착 버튼
    public TextMeshProUGUI actionButtonText;    // 앞면 버튼 텍스트
    public Button useButton;                    // 소모성 아이템 전용 사용 버튼

    [Header("Back (상세 정보 상태)")]
    public GameObject backFace;                 // 카드 뒷면 (상세 정보 UI)
    public TextMeshProUGUI itemNameText;        // 아이템 이름
    public TextMeshProUGUI priceText;           // 가격 텍스트
    public Slider speedSlider;                  // 속도 수치 슬라이더
    public Slider capacitySlider;               // 용량 수치 슬라이더
    public Slider efficiencySlider;             // 연비 수치 슬라이더
    public TextMeshProUGUI speedText;           // 속도 수치 텍스트
    public TextMeshProUGUI capacityText;        // 용량 수치 텍스트
    public TextMeshProUGUI efficiencyText;      // 연비 수치 텍스트
    public TextMeshProUGUI descriptionText;     // 설명 텍스트
    public Button actionButton_Back;            // 뒷면 구매/장착 버튼
    public TextMeshProUGUI actionButtonText_Back;   // 뒷면 버튼 텍스트

    [Header("Animation Settings")]
    [SerializeField] private float gaugeDuration = 1.0f;    // 게이지 애니메이션 연출 시간

    private PerformanceItemSO itemData;             // 아이템 데이터 참조
    private bool isFlipped = false;                 // 카드가 뒤집혔는지 여부
    private bool isFlipping = false;                // 현재 뒤집는 중인지 여부

    // 슬라이더 애니메이션을 제어하는 코루틴 참조
    private Coroutine speedCoroutine;
    private Coroutine capacityCoroutine;
    private Coroutine efficiencyCoroutine;

    // 아이템 슬롯에 데이터를 세팅하고 UI 초기 상태로 설정하는 함수
    public void Setup(PerformanceItemSO data)
    {
        itemData = data;                    // 아이템 데이터 저장

        itemImage.sprite = itemData.image;  // 슬롯의 대표 이미지 설정
        UpdateActionButton();               // 버튼 상태(구매/장착 등) 갱신

        // 상세 정보에 표시할 텍스트 설정
        itemNameText.text = itemData.DisplayName;
        priceText.text = $"{itemData.price}G";
        descriptionText.text = itemData.DisplayDescription;

        // 소모성 아이템 전용 버튼이 있을 경우, 초기에는 비활성화
        if (useButton != null)
            useButton.gameObject.SetActive(false);

        // 카드 뒷면을 숨기고 앞면만 보이도록 초기화
        isFlipped = false;
        frontFace.SetActive(true);
        backFace.SetActive(false);
        transform.localRotation = Quaternion.identity;
    }


    // 슬롯이 클릭되었을 때 호출됨. 카드가 회전 중이 아니면 뒤집기 코루틴 실행.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isFlipping) return;         // 이미 회전 중이면 무시
        StartCoroutine(FlipCard());     // 카드 뒤집기 코루틴 시작
    }

     // 카드 앞면/뒷면을 애니메이션으로 뒤집는 코루틴
    private IEnumerator FlipCard()
    {
        isFlipping = true;      // 회전 중 상태 플래그 설정

        float duration = 0.15f; // 회전 애니메이션 지속 시간
        float elapsed = 0f;     // 경과 시간 초기화
        Quaternion midRot = Quaternion.Euler(0f, 90f, 0f);  // 중간 회전 각도 (옆면)
        Quaternion endRot = isFlipped ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f);   // 목표 회전 각도 (앞→뒤 or 뒤→앞)
        Quaternion startRot = transform.localRotation;  // 시작 회전 상태 저장

        // 앞면 → 옆면으로 회전
        while (elapsed < duration)
        {
            transform.localRotation = Quaternion.Lerp(startRot, midRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = midRot;   // 정확히 옆면 고정

        // 앞면/뒷면 토글
        isFlipped = !isFlipped;
        frontFace.SetActive(!isFlipped);
        backFace.SetActive(isFlipped);

        if (isFlipped)
        {   
            // 뒷면 표시 시 회전 각도 고정 및 슬라이더 초기화
            backFace.transform.localRotation = Quaternion.Euler(0, 180f, 0);
            speedSlider.value = 0;
            capacitySlider.value = 0;
            efficiencySlider.value = 0;
            speedText.text = "0";
            capacityText.text = "0";
            efficiencyText.text = "0";

            // 기존 코루틴이 실행 중이면 중지
            if (speedCoroutine != null) StopCoroutine(speedCoroutine);
            if (capacityCoroutine != null) StopCoroutine(capacityCoroutine);
            if (efficiencyCoroutine != null) StopCoroutine(efficiencyCoroutine);

            // 슬라이더 애니메이션 코루틴 실행
            speedCoroutine = StartCoroutine(AnimateSlider(speedSlider, speedText, itemData.speed));
            capacityCoroutine = StartCoroutine(AnimateSlider(capacitySlider, capacityText, itemData.capacity));
            efficiencyCoroutine = StartCoroutine(AnimateSlider(efficiencySlider, efficiencyText, itemData.efficiency));
        }
        else
        {   
            // 앞면으로 돌아올 경우 회전 상태 초기화
            backFace.transform.localRotation = Quaternion.identity;
        }

        // 옆면 → 목표 방향으로 회전
        elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localRotation = Quaternion.Lerp(midRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = endRot;   // 최종 회전 고정
        isFlipping = false;                 // 회전 완료 플래그 해제
    }

    // 슬라이더와 텍스트를 애니메이션으로 서서히 채워주는 코루틴
    private IEnumerator AnimateSlider(Slider slider, TextMeshProUGUI valueText, float target)
    {
        float elapsed = 0f;

        while (elapsed < gaugeDuration) // 애니메이션이 끝날 때까지 반복
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / gaugeDuration);   // 경과 비율 (0~1)
            float value = Mathf.Lerp(0f, target, t);            // 선형 보간으로 현재 값 계산
            slider.value = value;                               // 슬라이더 값 갱신
            valueText.text = value.ToString("F1");              // 소수점 한 자리까지 텍스트 갱신
            yield return null;
        }

        // 최종 값 고정
        slider.value = target;
        valueText.text = target.ToString("F1");
    }

    // 버튼에 텍스트, 콜백, 상호작용 여부를 설정
    private void SetButtonState(Button button, TextMeshProUGUI text, UnityEngine.Events.UnityAction callback, string label, bool interactable)
    {
        text.text = label;                          // 버튼 텍스트 설정
        button.onClick.RemoveAllListeners();        // 기존 리스너 제거
        button.onClick.AddListener(callback);       // 새 콜백 등록
        button.interactable = interactable;         // 버튼 활성화 여부 설정
    }

    // 아이템의 소유/장착 상태에 따라 버튼 상태를 갱신
    public void UpdateActionButton()
    {
        if (itemData.itemType == ItemType.OneTime)
        {   
            // 일회성 아이템은 버튼을 비활성화
            actionButton.gameObject.SetActive(false);
            actionButton_Back.gameObject.SetActive(false);
            return;
        }

        bool isOwned = PerformanceInventoryManager.Instance.IsOwned(itemData);  // 보유 여부
        bool isEquipped = PerformanceInventoryManager.Instance.IsEquipped(itemData.category, itemData); // 장착 여부

        if (!isOwned)
        {   
            // 미보유 상태: "구매" 버튼 활성화
            SetButtonState(actionButton, actionButtonText, () => PerformanceShopManager.Instance.BuySelectedItem(itemData), "구매", true);
            SetButtonState(actionButton_Back, actionButtonText_Back, () => PerformanceShopManager.Instance.BuySelectedItem(itemData), "구매", true);
        }
        else
        {   
            // 보유 상태: "장착" or "장착중" 버튼 설정
            string label = isEquipped ? "장착중" : "장착";
            bool interactable = !isEquipped;

            SetButtonState(actionButton, actionButtonText, () => PerformanceShopManager.Instance.EquipSelectedItem(itemData), label, interactable);
            SetButtonState(actionButton_Back, actionButtonText_Back, () => PerformanceShopManager.Instance.EquipSelectedItem(itemData), label, interactable);
        }
    }

    // 소모성 아이템의 사용 버튼을 활성화하고, 클릭 이벤트 리스너를 설정
    public void EnableUseButton(UnityEngine.Events.UnityAction onClick)
    {
        if (useButton == null) return;              // 버튼이 없으면 아무 것도 하지 않음

        useButton.gameObject.SetActive(true);       // 버튼 표시
        useButton.onClick.RemoveAllListeners();     // 기존 리스너 제거
        useButton.onClick.AddListener(onClick);     // 새 리스너 추가
    }

    public void Refresh()
    {
        if (itemData == null) return;               // 아이템 데이터가 없으면 아무 것도 하지 않음

        bool isEquipped = PerformanceInventoryManager.Instance.IsEquipped(itemData.category, itemData); // 장착 여부 확인
        bool isOwned = PerformanceInventoryManager.Instance.IsOwned(itemData);                          // 보유 여부 확인

        // 앞면 버튼 설정
        if (actionButton != null)
        {
            if (!isOwned)
            {   
                // 미보유 상태: 구매 가능
                actionButton.interactable = true;
                actionButtonText.text = "구매";
                actionButton.onClick.RemoveAllListeners();
                actionButton.onClick.AddListener(() => PerformanceShopManager.Instance.BuySelectedItem(itemData));
            }
            else
            {   
                // 보유 중: 장착 또는 장착중 표시
                actionButton.interactable = true; // 항상 누를 수 있도록 설정
                actionButtonText.text = isEquipped ? "장착중" : "장착";

                actionButton.onClick.RemoveAllListeners();

                if (!isEquipped)
                    actionButton.onClick.AddListener(() => PerformanceShopManager.Instance.EquipSelectedItem(itemData));
            }
        }

        // 뒷면 버튼 설정
        if (actionButton_Back != null)
        {
            if (!isOwned)
            {   
                // 미보유 상태: 구매 가능
                actionButton_Back.interactable = true;
                actionButtonText_Back.text = "구매";
                actionButton_Back.onClick.RemoveAllListeners();
                actionButton_Back.onClick.AddListener(() => PerformanceShopManager.Instance.BuySelectedItem(itemData));
            }
            else
            {   
                // 보유 중: 장착 또는 장착중 표시
                actionButton_Back.interactable = true;
                actionButtonText_Back.text = isEquipped ? "장착중" : "장착";

                actionButton_Back.onClick.RemoveAllListeners();

                if (!isEquipped)
                    actionButton_Back.onClick.AddListener(() => PerformanceShopManager.Instance.EquipSelectedItem(itemData));
            }
        }
        
        // 디버그 로그로 현재 상태 출력
        Debug.Log($"[Refresh] {itemData.name} / isOwned: {isOwned}, isEquipped: {isEquipped}");

    }


}
