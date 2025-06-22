using UnityEngine;

public class PedestrianSpawner : MonoBehaviour
{
    [Header("스폰 간격 (초)")]
    public float spawnInterval = 3f;

    [Header("보행자 스폰 위치들")]
    public Transform[] spawnPoints;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnPedestrian), 0f, spawnInterval);
    }

    void SpawnPedestrian()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("Spawn Points가 설정되지 않았습니다.");
            return;
        }

        Vector3 spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;

        GameObject pedestrian = PedestrianPoolManager.Instance.GetFromPool(spawnPos);
        if (pedestrian == null)
        {
            Debug.LogWarning("보행자를 Pool에서 가져올 수 없습니다.");
        }
    }
}


/*
임시 pedestraian을 생성하기 위한 스크립트
게임 내의 spawnpoints의 position을 받아서 해당 위치에 보행자 주기적으로 생성함

이 스크립트는 PedestrianPoolManager를 통해 보행자를 풀에서 가져오고,
만약 풀에서 가져올 수 없다면 경고 메시지를 출력함
*/
