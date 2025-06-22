using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PerformanceShopManager : MonoBehaviour
{
    public static PerformanceShopManager Instance { get; private set; }

    [Header("탭별 패널")]
    [SerializeField] private Transform vehiclePanel;    // 차량 아이템 패널
    [SerializeField] private Transform consumablePanel; // 소모성 아이템 패널
    [SerializeField] private Transform oneTimePanel;    // 일회성 아이템 패널

    [Header("슬롯 프리팹")]
    [SerializeField] private GameObject vehicleSlotPrefab;  // 차량 슬롯 프리팹
    [SerializeField] private GameObject consumableSlotPrefab;   // 소모성 슬롯 프리팹
    [SerializeField] private GameObject oneTimeSlotPrefab;  // 일회성 슬롯 프리팹

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI moneyText; // 현재 돈 표시 텍스트
    [SerializeField] private TextMeshProUGUI turnStatusText;    // 남은 턴 표시 텍스트
    [SerializeField] private TextMeshProUGUI paymentAmountText; // 납부금 표시 텍스트
    [SerializeField] private Button payButton;                  // 납부 버튼
    private bool payButtonAssigned = false;
    [Header("게임 시작 버튼들")]
    [SerializeField] private GameObject gamePrepareButton;      // 게임 준비 버튼
    [SerializeField] private GameObject gameStartButton;        // 게임 시작 버튼

    [Header("아이템 데이터")]
    public PerformanceItemSO[] allItems;                        // 전체 아이템 목록

    private ShopTab currentTab = ShopTab.Vehicle;               // 현재 선택된 탭
    private bool isSubscribed = false;                          // 이벤트 중복 등록 방지용

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);  // 중복 인스턴스 제거
        else Instance = this;

        payButton.onClick.RemoveAllListeners(); // 기존 리스너 제거(중복 방지)
        payButton.onClick.AddListener(() => TryPayNextStage()); // 납부 버튼 클릭 시 처리
    }

    private void OnEnable()
    {
        if (!isSubscribed)
        {
            GameDataManager.OnDataLoaded += OnGameDataReady;    // 데이터 로딩 시 콜백 등록
            PerformanceInventoryManager.OnInventoryLoaded += RefreshAllSlots;   // 인벤토리 로딩 시 콜백 등록
            isSubscribed = true;
        }
        if (GameDataManager.Instance != null && GameDataManager.Instance.IsInitialized)
        {
            Debug.Log("OnDataLoaded 이벤트 놓쳐서 수동으로 호출");
            OnGameDataReady();  // 데이터가 이미 로드되어 있으면 수동 호출
        }

        // 리스너는 오직 1번만 등록되도록 중복 방지
        if (!payButtonAssigned)
        {
            payButton.onClick.RemoveAllListeners(); // 혹시 남아있는 걸 제거
            payButtonAssigned = true;
        }

        gamePrepareButton.SetActive(true);  // 준비 버튼 활성화
        gameStartButton.SetActive(false);   // 시작 버튼 비활성화
    }

    private void OnDisable()
    {
        if (isSubscribed)
        {
            GameDataManager.OnDataLoaded -= OnGameDataReady;
            PerformanceInventoryManager.OnInventoryLoaded -= RefreshAllSlots;
            isSubscribed = false;
        }
    }

    private void OnGameDataReady()
    {
        Debug.Log("🚨 OnGameDataReady 호출됨");
        vehiclePanel.gameObject.SetActive(true);    // 초기 탭은 차량 탭
        consumablePanel.gameObject.SetActive(false);
        oneTimePanel.gameObject.SetActive(false);

        UpdateMoneyUI();    // 보유 돈 UI 업데이트
        UpdateTurnAndPaymentUI();   // 턴/납부 금액 UI 업데이트
        GenerateShopSlots();    // 상점 슬롯 생성
    }

    public void OnTabSelected(int tabIndex)
    {
        currentTab = (ShopTab)tabIndex; // 선택된 탭 저장

        vehiclePanel.gameObject.SetActive(currentTab == ShopTab.Vehicle);
        consumablePanel.gameObject.SetActive(currentTab == ShopTab.Consumable);
        oneTimePanel.gameObject.SetActive(false);   // 일회성은 직접 표시하지 않음

        GenerateShopSlots();    // 해당 탭의 슬롯 다시 생성
    }

    private void GenerateShopSlots()
    {
        Transform targetPanel = GetCurrentPanel();  // 현재 탭에 해당하는 패널을 가져옴

        foreach (Transform child in targetPanel)
            Destroy(child.gameObject);  // 기존 슬롯 제거

        foreach (var item in allItems)
        {   
            // 디버깅용 로그: 아이템 이름, ID, 보유 여부 출력
            Debug.Log($"[CHECK] item.name = {item.name}, itemId = {item.itemId}, isOwned: {PerformanceInventoryManager.Instance.IsOwned(item)}");
            if (!IsItemInCurrentTab(item)) continue;     // 해당 탭의 아이템만 표시

            var prefab = GetPrefabForItem(item.itemType);   // 아이템 타입별 슬롯 프리팹
            var slot = Instantiate(prefab, targetPanel);    // 슬롯 프리팹을 현재 패널에 인스턴스화

            // 일반 슬롯 인터페이스를 구현한 경우만 처리
            if (slot.TryGetComponent(out PerformanceItemSlot generalSlot))
            {
                generalSlot.Setup(item);    // 슬롯에 아이템 데이터 초기화

                // 소모성 아이템의 경우 '사용' 버튼 기능 설정
                if (item.itemType == ItemType.Consumable)
                {
                    generalSlot.EnableUseButton(() =>
                    {   
                        // 현재 보유 중인 해당 아이템을 찾아서
                        var ownedItem = GameDataManager.Instance.data.ownedItems.Find(x => x.itemId == item.itemId);
                        if (ownedItem != null && ownedItem.count > 0)
                        {
                            ownedItem.count--;  // 개수 감소

                            // 0개 이하가 되면 목록에서 제거
                            if (ownedItem.count <= 0)
                                GameDataManager.Instance.data.ownedItems.Remove(ownedItem);

                            GameDataManager.Instance.Save();    // 변경된 데이터 저장
                            PerformanceInventoryManager.Instance.LoadFromGameData(GameDataManager.Instance.data);   // 인벤토리 다시 불러오기
                        RefreshAllSlots(); // UI 슬롯 새로고침
                            RefreshAllSlots();
                        }
                    });
                }
            }
        }

        // 즉시 레이아웃 다시 계산하여 UI 정렬 반영
        LayoutRebuilder.ForceRebuildLayoutImmediate(targetPanel.GetComponent<RectTransform>()); // 레이아웃 갱신
    }

    // 현재 선택된 탭에 해당하는 패널을 반환
    private Transform GetCurrentPanel()
    {
        return currentTab switch
        {
            ShopTab.Vehicle => vehiclePanel,        // 차량 탭이면 vehiclePanel 반환
            ShopTab.Consumable => consumablePanel,  // 소모성 탭이면 consumablePanel 반환
            _ => vehiclePanel                       // 기본값으로 vehiclePanel 반환
        };
    }

    // 주어진 아이템이 현재 탭에 속한 아이템인지 판별
    private bool IsItemInCurrentTab(PerformanceItemSO item)
    {
        return currentTab switch
        {
            ShopTab.Vehicle => item.itemType == ItemType.Permanent || item.itemType == ItemType.Vehicle,    // 차량 탭일 경우 영구 아이템이나 차량 타입 
            ShopTab.Consumable => item.itemType == ItemType.Consumable,                                     // 소모성 탭일 경우 소모성 아이템만
            _ => false,                                                                                     // 그 외는 탭에 포함되지 않음
        };
    }

    public void BuySelectedItem(PerformanceItemSO item)
    {
        if (PerformanceInventoryManager.Instance.IsOwned(item)) return; // 이미 소유한 경우
        if (GameDataManager.Instance.data.money < item.price)
        {
            Debug.Log("골드 부족");
            return;
        }

        GameDataManager.Instance.data.money -= item.price;  // 돈 차감
        PerformanceInventoryManager.Instance.BuyItem(item); // 구매 처리
        UpdateMoneyUI();
        RefreshAllSlots();
    }

    public void EquipSelectedItem(PerformanceItemSO item)
    {
        PerformanceInventoryManager.Instance.EquipItem(item.category, item);    // 장착 처리
        RefreshAllSlots();
    }

    // 현재 선택된 탭의 모든 슬롯을 새로고침하는 함수
    private void RefreshAllSlots()
    {
        Debug.Log("RefreshAllSlots 호출됨");

        // 현재 탭에 해당하는 패널을 가져옴
        Transform targetPanel = GetCurrentPanel();

        // 패널의 자식들(슬롯들)을 순회하며
        foreach (Transform child in targetPanel)
        {   
            // PerformanceItemSlot 컴포넌트를 가진 경우만 Refresh 호출
            if (child.TryGetComponent(out PerformanceItemSlot slot))
                slot.Refresh(); // 슬롯 UI 새로고침 (보유 상태, 버튼 상태 등 갱신)
        }
    }

    // 상단의 돈 UI를 현재 데이터 기준으로 갱신
    public void UpdateMoneyUI()
    {   
        // GameDataManager에서 현재 돈을 가져와 텍스트에 반영
        moneyText.text = GameDataManager.Instance.data.money + "원";    // 돈 텍스트 갱신
    }

    // 상단의 남은 턴 수와 납부 금액 UI를 갱신
    public void UpdateTurnAndPaymentUI()
    {
        var data = GameDataManager.Instance.data;

        int remainingTurns = data.turn; // 현재 남은 턴
        int maxTurns = data.maxTurnsPerRound;   // 최대 턴 수
        string color = remainingTurns <= 1 ? "#FF5555" : "#55FF55"; // 남은 턴이 1 이하이면 빨간색, 아니면 초록색

        turnStatusText.text = $"<color={color}>남은 턴: {remainingTurns} / {maxTurns}</color>"; // 남은 턴 수 텍스트 업데이트 (색상 포함)
        paymentAmountText.text = $"집세 : {GameDataManager.Instance.GetRequiredPayment()}원";   // 납부 금액 텍스트 업데이트
    }

    // 다음 라운드를 위한 집세 납부 시도 함수
    public void TryPayNextStage()
    {   
        // 디버깅 로그: 현재 돈 출력
        Debug.Log($"[TryPayNextStage] 호출됨 - money: {GameDataManager.Instance.data.money}");

        // 납부 시도
        bool success = GameDataManager.Instance.TryPay();

        // 납부 성공 시
        if (success)
        {
            // 턴 수를 다시 최대 턴 수로 초기화
            GameDataManager.Instance.data.turn = GameDataManager.Instance.data.maxTurnsPerRound;    // 턴 초기화

            // 데이터 저장
            GameDataManager.Instance.Save();

            // UI 갱신
            UpdateMoneyUI();
            UpdateTurnAndPaymentUI();
        }
        else
        {   
            // 실패 시 로그 출력(디버그용)
            Debug.Log("돈이 부족합니다.");
        }
    }


    public void OnGameStartButtonClicked()
    {
        vehiclePanel.gameObject.SetActive(false);   // 차량 탭 숨김
        consumablePanel.gameObject.SetActive(false);    // 소모성 탭 숨김

        ShowOneTimeItemSelection(); // 일회성 아이템 선택 화면 활성화
        oneTimePanel.gameObject.SetActive(true);    // 일회성 패널 표시
    }
    public void OnOneTimeConfirmButtonClicked()
    {
        ApplySelectedOneTimeItems();    // 선택한 아이템 적용

        // 시작 버튼 숨기기
        gameStartButton.SetActive(false);   // 게임 시작 버튼 숨김
        oneTimePanel.gameObject.SetActive(false);   // 일회성 아이템 패널 숨김

        StartCoroutine(DelayedStartGame()); // 게임 시작 코루틴 호출
    }


    private IEnumerator DelayedStartGame()
    {
        yield return null;  // 1프레임 대기
        Debug.Log("게임 시작!");
        // SceneManager.LoadScene("GameScene");
    }

    private void ShowOneTimeItemSelection()
    {
        foreach (Transform child in oneTimePanel)
            Destroy(child.gameObject);   // 기존 슬롯 제거

        foreach (var item in allItems)
        {
            if (item.itemType != ItemType.OneTime) continue;    // 일회성 아이템만 대상

            var slot = Instantiate(oneTimeSlotPrefab, oneTimePanel);    // 슬롯 프리팹 생성
            if (slot.TryGetComponent(out PerformanceOneTimeSlot oneTimeSlot))
                oneTimeSlot.Setup(item);    // 슬롯에 아이템 데이터 설정
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(oneTimePanel.GetComponent<RectTransform>());    // 레이아웃 갱신
    }

    public void ApplySelectedOneTimeItems()
    {   
        // 현재 씬에서 모든 일회성 슬롯을 가져옴
        var oneTimeSlots = Object.FindObjectsByType<PerformanceOneTimeSlot>(FindObjectsSortMode.None);

        foreach (var slot in oneTimeSlots)
        {
            if (!slot.IsSelected) continue; // 선택되지 않은 슬롯은 무시

            var data = slot.GetItemData();   // 아이템 데이터 획득
            if (GameDataManager.Instance.data.money >= data.price)
            {
                GameDataManager.Instance.data.money -= data.price;  // 돈 차감
                GameDataManager.Instance.data.ownedItems.Add(new SerializableItem
                {
                    itemId = data.name,
                    itemType = ItemType.OneTime,
                    count = 1,
                    isUnlocked = true,
                    isEquipped = false
                });

                Debug.Log($"{data.DisplayName} 선택됨 - {data.price}원 차감됨");
            }
        }

        GameDataManager.Instance.Save();    // 저장
        UpdateMoneyUI();    // 돈 UI 갱신
    }

    private GameObject GetPrefabForItem(ItemType itemType)
    {   
        // 아이템 타입에 따라 적절한 프리팹 반환
        return itemType switch
        {
            ItemType.Consumable => consumableSlotPrefab,
            ItemType.OneTime => oneTimeSlotPrefab,
            _ => vehicleSlotPrefab
        };
    }
    private void OnDataReloadedExternally()
    {
        Debug.Log("외부에서 데이터가 갱신됨, 상점 UI 다시 생성");
        OnGameDataReady(); // 전체 UI 초기화 및 재구성
    }
    
    public void OnGamePrepareButtonClicked()
    {
        gamePrepareButton.SetActive(false);  // 준비 버튼 숨김
        gameStartButton.SetActive(true);     // 시작 버튼 표시

        // 다른 패널 숨기고 일회성 선택 화면 표시
        vehiclePanel.gameObject.SetActive(false);
        consumablePanel.gameObject.SetActive(false);
        ShowOneTimeItemSelection();
        oneTimePanel.gameObject.SetActive(true);
    }

}
