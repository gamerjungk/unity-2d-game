using UnityEngine;
using System.Collections.Generic;
using System.IO;

// -------------------- 아이템 종류(아이템 타입) 열거형 --------------------
public enum ItemType
{
    Consumable,     // 소모형 (예: 연료, 치료제)
    OneTime,        // 일회성 잠금 해제 (예: 특정 퀘스트)
    Permanent,      // 영구형 (예: 차량, 스킨)
    Vehicle,        // 탈것
    Skin,           // 외형
    Currency        // 재화 (예: 골드)
}

[System.Serializable]
public class SerializableItem
{
    public string itemId;           // 아이템 고유 ID 또는 ScriptableObject의 이름
    public int count;               // 보유 수량 (소모형의 경우 중요)
    public ItemType itemType;       // 아이템 분류 (소모형, 영구형 등)
    public bool isEquipped;         // 장착 여부 (장비/스킨 등 적용 상태)
    public float duration;          // 유효 시간 (초 단위, 예: 임시 버프 등)
    public bool isUnlocked;         // 잠금 해제 여부 (OneTime 전용 플래그)
}
[System.Serializable]
public class GameData
{
    //재화 및 턴
    public int gold;    // 골드 (게임 외적 재화)
    public int money;   // 화폐 (인게임 재화)
    public int turn;    // 남은 턴 수

    // 해당 납부 단계에서 최대 플레이 가능한 라운드 수
    public int maxTurnsPerRound = 5;

    //납부 관련
    public int currentRound;    // 현재 라운드
    public int paidStageIndex;  // 현재까지 납부 완료한 단계 인덱스

    public List<SerializableItem> ownedItems = new List<SerializableItem>();    // 저장 가능한 아이템 목록
}

// -------------------- 게임 저장/불러오기 처리 클래스 --------------------
public static class SaveManager
{
     // 저장 파일 경로 설정 (persistentDataPath는 OS별 영구 저장 경로)
    private static string SavePath => Application.persistentDataPath + "/save.json";

    // ---------------- 저장 ----------------
    public static void Save(GameData data)
    {
        // GameData 객체를 JSON 문자열로 변환 (들여쓰기 포함)
        string json = JsonUtility.ToJson(data, true);

        // 변환된 JSON 문자열을 파일로 저장
        File.WriteAllText(SavePath, json);
    }

    // ---------------- 불러오기 ----------------
    public static GameData Load()
    {
        // 파일이 없으면 새로운 GameData 반환 (초기값)
        if (!File.Exists(SavePath))
            return new GameData();

        // 저장된 JSON 파일을 읽고 GameData 객체로 역직렬화
        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<GameData>(json);
    }
}
