using UnityEngine;
using System;
using System.IO;     // 파일 저장 및 삭제
using System.Collections;   // 코루틴 사용
using System.Collections.Generic;   // List 등 컬렉션 사용
using UnityEngine.SceneManagement;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }    // 싱글턴 인스턴스

    public static event Action OnDataLoaded; // 데이터 최초 로딩 완료 시 이벤트
    public static event Action OnDataReloaded;  // 데이터 강제 리로드(리셋 포함) 시 이벤트
    public bool IsInitialized { get; private set; } = false;    // 초기화 여부 플래그

    public GameData data;   // 실제 저장되고 불러오는 데이터

    private void Awake()
    {
        // 싱글턴 중복 방지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지되도록 설정

        Load(); // 앱 시작 시 바로 데이터 로드
    }


    private void Start()
    {
        StartCoroutine(DelayedInit());  // 다른 시스템보다 늦게 초기화되도록 코루틴 시작
    }

    private IEnumerator DelayedInit()
    {
        // 다른 시스템보다 늦게 실행되도록 1프레임 대기
        yield return null;
        
        // PerformanceInventoryManager.Instance?.LoadFromGameData(data);
        IsInitialized = true;   // 초기화 완료 플래그 설정

        // 디버그용 코드
        Debug.Log("GameDataManager 초기화 완료");   
        OnDataLoaded?.Invoke(); // 데이터 로딩 완료 이벤트 호출
    }

    // 저장 함수
    public void Save()
    {
        SaveManager.Save(data); // 저장 로직은 SaveManager에서 처리
        Debug.Log("저장경로: " + Application.persistentDataPath);   // 디버그용 로그
    }

    // 불러오기 함수
    public void Load()
    {
        data = SaveManager.Load();  // 저장된 데이터 로드

        // 처음 실행이거나 ownedItems가 비어 있으면 초기값 설정
        if (IsFirstPlay() || data.ownedItems == null || data.ownedItems.Count == 0)
        {
            Debug.Log("최초 실행 - 기본값 세팅");
            data.gold = 100;
            data.money = 10000;
            data.turn = 5;
            data.paidStageIndex = 0;
            data.currentRound = 1;
            data.ownedItems = new List<SerializableItem>();

            Save(); // 기본값 저장
            PlayerPrefs.SetInt("HasPlayed", 1); // 첫 실행 여부 저장

            OnDataReloaded?.Invoke(); // 강제 로딩 이벤트 호출
        }

        // 인벤토리 시스템에 데이터 전달
        PerformanceInventoryManager.Instance?.LoadFromGameData(data);
    }

    
    private bool IsFirstPlay() => PlayerPrefs.GetInt("HasPlayed", 0) == 0;// 최초 실행인지 확인
    private void OnApplicationPause(bool pause) { if (pause) Save(); }  // 앱 백그라운드 진입 시 자동 저장
    private void OnApplicationQuit() { Save(); }    // 앱 종료 시 자동 저장

    // 현재 납부 금액 계산 (스테이지에 따라 점점 증가)
    public int GetRequiredPayment()
    {
        return 1000 + (data.paidStageIndex * 1000);
    }

    // 납부 시도 → 성공하면 돈 차감 및 단계 증가
    public bool TryPay()
    {
        int currentIndex = data.paidStageIndex;
        int required = 1000 + (currentIndex * 1000); // 납부 금액 계산

        Debug.Log($"납부 시도 - 현재 paidStageIndex: {currentIndex}, 납부액: {required}, 현재 보유금액: {data.money}");

        if (data.money >= required)
        {
            data.money -= required; // 돈 차감
            data.paidStageIndex++;  // 납부 단계 증가

            Debug.Log($"납부 성공. 남은 금액: {data.money}, 다음 paidStageIndex: {data.paidStageIndex}");

            SaveManager.Save(data); // 저장
            return true;
        }
        Debug.Log("돈 부족으로 납부 실패");
        return false;
    }

    // 라운드 종료 시 일회성 아이템 제거
    public void ClearOneTimeItems()
    {
        int before = data.ownedItems.Count;
        data.ownedItems.RemoveAll(item => item.itemType == ItemType.OneTime); // 일회성 제거
        int after = data.ownedItems.Count;
        Debug.Log($"🧹 라운드 종료로 일회성 아이템 {before - after}개 제거됨");
    }

    // 0903 추가
    // 라운드 종료를 처리하는 UI/GameManager 호출
    public void EndRoundCleanup()
    {
        // 1) 저장 데이터에서 일회용 아이템 제거
        ClearOneTimeItems();

        // 2) 다시 퍼포먼스 인벤토리에 반영
        PerformanceInventoryManager.Instance.LoadFromGameData(data);

        // 3) 저장
        Save();
    }

    // 전체 게임 데이터 초기화 (세이브 삭제 + 인벤토리 초기화) 험슈
    public void ResetGameData()
    {
        string path = Application.persistentDataPath + "/save.json";
        if (File.Exists(path))
        {
            File.Delete(path);  // 저장 파일 삭제
            Debug.Log("🗑️ save.json 삭제됨");
        }

        PlayerPrefs.DeleteAll();    // 모든 PlayerPrefs 초기화
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs 초기화됨");

        PerformanceInventoryManager.Instance.ClearAll();

        Load(); // 기본값으로 재설정
        OnDataReloaded?.Invoke(); // 강제 로딩 이벤트 호출
    }
    
    // 돈 추가
    public void AddMoney(int amount)
    {
        data.money += amount;
        Debug.Log("현재 돈: " + data.money);
    }
    
    // 돈 차감
    public void SubMoney(int amount)
    {
        data.money -= amount;
        Debug.Log("현재 돈: " + data.money);
    }
}
