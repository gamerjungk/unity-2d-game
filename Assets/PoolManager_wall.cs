using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class PoolManager_wall : MonoBehaviour
{
    public static PoolManager_wall Instance;
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        Instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary[pool.tag] = objectPool;
        }
    }

    public GameObject GetFromPool(string poolTag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.TryGetValue(poolTag, out var poolQueue) || poolQueue.Count == 0)
        {
            Debug.LogWarning($"Pool with tag '{poolTag}' is empty or does not exist.");
            return null;
        }

        GameObject obj = poolQueue.Dequeue();
        obj.SetActive(true);
        obj.transform.SetPositionAndRotation(position, rotation);
        return obj;
    }

    public void ReturnToPool(string poolTag, GameObject obj)
    {
        if (!poolDictionary.TryGetValue(poolTag, out var poolQueue))
        {
            Debug.LogWarning($"Pool with tag '{poolTag}' does not exist.");
            Destroy(obj); // 안전조치: 해당 풀이 없으면 제거
            return;
        }

        obj.SetActive(false);
        poolQueue.Enqueue(obj);
    }
}