using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
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

    public GameObject Spawn(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        List<GameObject> pool = poolDictionary[tag];
        GameObject objectToSpawn = null;

        // ��Ȱ��ȭ�� ������Ʈ ã�Ƽ� ����
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                objectToSpawn = obj;
                break;
            }
        }

        // �� ã������ ���� ����
        if (objectToSpawn == null)
        {
            objectToSpawn = Instantiate(prefabLookup[tag]);
            pool.Add(objectToSpawn); // ���� ���� ������Ʈ�� Ǯ�� ���
        }

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        return objectToSpawn;
    }
}
