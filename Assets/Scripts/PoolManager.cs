using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // 자주 생성할 오브젝트를 등록해두고 Spawn 함수를 통해 생성해서 효율적인 복제
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
    }

    public static PoolManager Instance;

    public List<Pool> pools;
    private Dictionary<string, List<GameObject>> poolDictionary;
    private Dictionary<string, GameObject> prefabLookup;

    void Awake()
    {
        Instance = this;
        poolDictionary = new Dictionary<string, List<GameObject>>();
        prefabLookup = new Dictionary<string, GameObject>();

        foreach (Pool pool in pools)
        {
            poolDictionary[pool.tag] = new List<GameObject>();
            prefabLookup[pool.tag] = pool.prefab;
        }
    }

    // 특정 tag를 가진 오브젝트를 position위치에 rotation 회전각으로 생성
    public GameObject Spawn(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        List<GameObject> pool = poolDictionary[tag];
        GameObject objectToSpawn = null;

        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                objectToSpawn = obj;
                break;
            }
        }

        if (objectToSpawn == null)
        {
            objectToSpawn = Instantiate(prefabLookup[tag]);
            pool.Add(objectToSpawn); 
        }

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        return objectToSpawn;
    }
}