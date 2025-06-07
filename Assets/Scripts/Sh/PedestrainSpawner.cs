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
